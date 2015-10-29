using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DominoTable13
{
    public delegate void SetFunction(int Value);
    public delegate void SetText(String text); // тип функции, который отображает данные на экране в главном окне

    // Суммы домино в таблице MaxY*MaxX (соответствует таблице 2*MaxY*MaxX чисел):
    // 1. Горизонтально (0...MaxX-1) - MaxX рядов костяшек.
    // 2. Вертикально (0...(MaxY*2-1)) - считаем не костяшки, а числа - два числа на костяшку. Итго шесть строк чисел в вертикальном ряду из трех костяшек.
    // 3. Диагонали: DU - слева на право, снизу вверх; UD - слева на право, сверху вниз.
    
    class Domino
    {
        public byte top; // top of the domino
        public byte bottom; // bottom of the domino
        public byte sum; // top + bottom
        public byte num; // порядковый номер домино
        public bool rot; // положение костяшки false - обычное, true - top и bottom поменяли местами
        public bool IsNotSimmetric; // симметричную костяшку нет смысла вращать. Истина - можно вращать. ЛОжь - нет смысла.
        public bool Empty; // пустая не инициализированная костяшка
        public Domino()
        {
            top = 0;
            bottom = 0;
            sum = 0;
            num = 0;
            rot = false;
            IsNotSimmetric = false;
            Empty = true;
        }
        public Domino(byte top, byte bottom, byte num)
        {
            this.top = top;
            this.bottom = bottom;
            this.num = num;
            this.sum = (byte)(top + bottom);
            this.rot = false;
            this.IsNotSimmetric = (top != bottom);
            Empty = false;
        }
        public bool IsEmpty()
        {
            return Empty;
        }
        // задать положение костяшки. true - основное. false - повернутое.
        public void SetOrder(bool MainOrder=true)
        {
            if (Empty) return;
            if (MainOrder != rot) Rotate();
        }
        // повернуть костяшку отностительно текущего положения
        public void Rotate()
        {
            //if (Empty || !IsNotSimmetric) return;
            byte temp = top;
            top = bottom;
            bottom = temp;
            rot = !rot;
        }
        public override String ToString()
        {
            String text = (top.ToString() + bottom.ToString()); //(rot ? 'r':' ') + (top.ToString() + bottom.ToString());
            return text;
        }
    }

    // представляе таблицу домино MaxX ряда по MaxY домино.
    // позволяет найти нужное сочетание домино, так что сумма чисел каждого ряда (верх и низ костяшки отдельно),
    // колоки и диагоналей == CRC
    class DominoTable
    {
        // Костяшки домино выстраиваются прямоугольником MaxX*MaxY. Длинная сторона костяшки распологается по вертикали.
        // x|x|x|x|x|x| 0  ^   0 <- top
        // x|x|x|x|x|x|    |   1 <- bottom
        // - - - - - -     |
        // x|x|x|x|x|x| 1  |   2 <- top
        // x|x|x|x|x|x|    у   3 <- bottom
        // - - - - - -     |
        // x|x|x|x|x|x| 2  |   4 <- top
        // x|x|x|x|x|x|    |   5 <- bottom
        // - - - - - -     V
        // 0 1 2 3 4 5
        // <--- x --->

        byte MaxX; // ширина таблицы (кол-во рядов) == MaxY*2
        byte MaxY; // высота таблицы в костяшках домино (чисел в два раза больше)

        byte MaxN; // максимальная позиция == MaxX*MaxY - 1
        // Вначале изменяется y[0..MaxY-1] - он маленький и по вертикали быстрее всего считается сумма
        // Потом изменяется x[0..MaxX-1]
        
        Domino[] NumDomino; // Для быстрого получения домино по его порядковому номеру 0..MaxI-1

        byte MaxI; // максимальный номер домино, если считать их от 0.
        byte CRC; // контрольная сумма числе костяшек. проверяется по вертикали, горизонтали и главным диагоналям.
        byte startI; // начальный код костяшки домино на позиции (0,0). С нее начинается проверка. Нужно для распараллеливания процесса.
        byte endI; // конечный код костяшки домино на позиции (0,0). Ей заканчивается проверка. Нужно для распараллеливания процесса.
   
        int RefreshTo; // интервал обновления отладочной информации

        bool NotSolve; // решений нет

        SetText SetInfo; // ссылка на функцию отрисовки информации о текущем состоянии поиска решения
        SetText SetTableText; // ссылка на функцию отрисовки таблицы домино

        public DominoTable(byte MaxY, byte CRC, byte startI, byte endI, SetText SetInfo, SetText SetTableText, int RefreshTo = 1000000000)
        {
            this.MaxY = MaxY;
            this.MaxX = (byte)(MaxY * 2);
            this.MaxN = (byte)(MaxX * MaxY);

            this.CRC = CRC;
            this.startI = startI;
            
            this.RefreshTo = RefreshTo;

            this.SetInfo = SetInfo;
            this.SetTableText = SetTableText;

            NotSolve = true;

            NumDomino = new Domino[28]; // Больше чем 28 костяшек домино от (0;0) до (6;6) нет.

            byte num = 0;
            for(byte top = 0; top < 7; top++)
            {
                for (byte bottom = top; bottom < 7; bottom++)
                {
                    //if (top + bottom > 8) continue; // нам не подойдут домино с суммой чисел больше 8
                    NumDomino[num] = new Domino(top, bottom, num);
                    num++;
                }
            }
            MaxI = num;

            if (endI == 0) this.endI = MaxI; // определяем если нужно максимальное значение
            else this.endI = endI;
        }
        // переопределяем размер таблицы домино
        public void Reset(byte MaxY)
        {
            if (MaxY == this.MaxY) return; // Размер не изменился

            this.MaxY = MaxY;
            this.MaxX = (byte)(MaxY * 2);

            NotSolve = true;
        }

        public String GetAllDominoText()
        {
            String text = "";
            byte LastTop = 0;

            for (byte t = 0; t < MaxI; t++)
            {
                Domino d = NumDomino[t];

                if (d.top != LastTop) text += Environment.NewLine;
                text = text + "(" + d.top + "," + d.bottom + "," + t.ToString("D2") + ")";
                
                LastTop = d.top;
            }

            return text;
        }

        public override String ToString()
        {
            String text = "Y" + MaxY.ToString() + "X" + MaxX.ToString() + "CRC" + CRC.ToString() + "Start" + startI.ToString() + "End" + endI.ToString();
            return text;
        }

        // Найти такую таблицу из домино (MaxY строки в MaxY*2 рядов), что суммы по вертикали, горизонтали и главным диагоналям будет == CRC.
        public void Solve()
        {
            if(MaxI == 0) return; // максимальный номер костяшки домино: 0 - (0;0), 1 - (0;1), ... 27 - (6;6).

            int iRefresh = 0; // итератор для определения, когда выводить промежуточную информацию
            UInt64 count = 0; // количество итераций
            int NumSolv = 0; // количество найденных решений
            
            // n - текущая обрабатываемая позиция в таблице == x + y*MaxX
            // Кроме позиции домино n (0..MaxN-1) используется ориентация костяшек false, true (см. Domino.rot)
            byte n = 0;
            byte x = 0;
            byte y = 0;

            //текущий номер домино, вставляемый в позицию n слова. Изменяется от 0...(MaxI-1).
            byte i = 0;

            //отмечаем уже используемые домино
            bool[] used = new bool[MaxI];

            Domino[] d = new Domino[MaxN]; // массив расставленных домино, их использовать нельзя

            // разделитель строк костяшек домино. для отрисовки на экране.
            String divline = "";
            for (byte t = 0; t < MaxX; t++) divline += "- ";

            Summs Summs1 = new Summs(MaxX,MaxY, CRC); // объект для подсчета контрольных сумм по рядам, строкам и главным диагоналям.
            byte code = 0; // код проверки CRC. если 0 - то все ок и в Summs1 учтена текущая костяшка. иначе в Summs1 нет текущей костяшки (удалять ее из суммы не нужно).

            NotSolve = true; // нет решения.
            
            DateTime StartTime = DateTime.Now;

            String threadName = "Поток " + startI.ToString("D2") + ":" + endI.ToString("D2") + "| ";
            { // Отладочная информация
                String str = Environment.NewLine + threadName + "Число домино: " + MaxI.ToString() + Environment.NewLine;
                str += threadName + "Начинаем обработку " + StartTime.ToString("yyyy.MM.dd hh:mm:ss-ff") + Environment.NewLine;
                SetTableText(str);
                //SetInfo("");
            }

            bool NotNull = false; // есть ли значение в d[n]

            DateTime Now = DateTime.Now;

            // Устанавливаем начальное значение
            i = startI;

            // В цикле пока не найдем решение.
            while (true)
            {
                count++;
                
                if (iRefresh == RefreshTo)
                {
                    // выводим диагностику
                    iRefresh = 0;
                    
                    String debugtxt = threadName + (DateTime.Now - Now).TotalMilliseconds.ToString("00000000") + " ";
                    debugtxt += count.ToString("D12") + ": ";
                    for (byte tn = 0; tn < MaxN && d[tn] != null; tn++)
                    {
                        debugtxt += d[tn].ToString() + "|";
                    }
                    debugtxt += Environment.NewLine;

                    SetTableText(debugtxt);

                    Now = DateTime.Now;
                }
                else
                    iRefresh++;


                // Обрабатываем костяшку в позиции n: либо поворачиваем костяшку, если она уже установлена, либо ищем подходящую из свободных.
                // подбираем первый подходящий номер i
                if (NotNull && code != 1 && d[n].IsNotSimmetric && !d[n].rot)
                {
                    if(code == 0)
                        Summs1.RemoveDomino(x, y, ref d[n]); // удаляем из контрольных сумм, данные текущей костяшки.
                    d[n].Rotate();
                }
                else
                {
                    if (NotNull) // На этом месте уже стоит повернутая костяшка. Убираем ее.
                    {
                        if (code == 0) Summs1.RemoveDomino(x, y, ref d[n]); // удаляем из контрольных сумм, данные текущей костяшки.
                        if (d[n].rot) d[n].Rotate(); // она повернута, вернем в исходное состояние.
                        i = d[n].num; // запоиминаем номер убираемой костяшки.
                        used[i] = false; // отмечаем что убираемая костяшка не используется.
                        d[n] = null; // очищаем позицию домино.
                        NotNull = false; // в текущей позиции d[n] ничего нет
                        i++; // была костяшка номер num. ищем слещующую по номеру не использованную.
                    }

                    // ищем следующую свободную костяшку
                    while (i < MaxI && used[i]) i++;

                    if ((n > 0 && i < MaxI) || (n == 0 && i < endI)) // граница перебора костяшек, либо до endI в первой позиции, либо любая костяшка
                    {
                        //помещаем костяшку с номером i в текущую позицию n
                        d[n] = NumDomino[i];
                        NotNull = true; // в текущей позиции d[n] установлена костяшка
                        used[i] = true;
                    }
                    else // смещаемся на предыдущую позицию, т.к. эту заполнить не получилось.
                    {
                        if ((endI == 0 && n == 0) || (endI > 0 && n == 1)) // здесь еще не сместились, но считаем, что сместились.
                        {
                            Now = DateTime.Now;
                            SetInfo(threadName + "Обработка закончена");
                            SetTableText(threadName + " Тек. время: " + Now.ToString("yyyy.MM.dd hh:mm:ss-ff") + ". Прошло млсек.: " +
                                (Now - StartTime).TotalMilliseconds.ToString("00000000") + Environment.NewLine);
                            break; // перебрали все возможные варианты, заканчиваем обработку.
                        }
                        
                        n--;

                        if (y == 0) // дальше смещаться уже некуда
                        {
                            x--;
                            y = (byte)(MaxY - 1);
                        }
                        else y--;

                        i = 0;
                        code = 0; // чтобы удалялись из контрльных сумм костяшки в предыдущих позициях.
                        NotNull = true; // в текущей позиции d[n] установлена костяшка

                        continue; // заходим на проверку новых перестановок в предыдущей позиции.
                    }
                }

                // Здесь у нас в позиции n == (x,y) есть новая костяшка (или ее позиция) d[n].
                // Проверяем условия сумм по горизонтали, вертикали и диагоналям
                code = Summs1.AddDomino(x, y, ref d[n]); // рассчитываем суммы и проверяем их

                if (code > 0) // не прошли по контрольной сумме. из контрольных сумм костяшка исключена!
                {
                     //if (code == 1 && !d[n].rot && d[n].IsNotSimmetric)
                     //   d[n].Rotate(); // проскакиваем итерацию цикла с поворотом костяшки.

                    continue; // здесь code обнулять не нужно. на следущей итерации цикла поиска, данная костяшка не будет исключена из контрльных сумм, так как это уже сделано.
                }

                // Позиция n заполнена подходящей домино. Переходим к следующей позиции.
                n++;
                y++;
                if(y >= MaxY)
                {
                    y = 0;
                    x++;
                }
                i = 0;
                NotNull = false; // в следующей позиции не стоит костяшка.

                if (n == MaxN) // конец слова: все домино расставлены
                {
                    // отображаем слово
                    NotSolve = false;
                    NumSolv++;

                    Now = DateTime.Now;
                    String text = threadName + "!!! Нашли решение №" + NumSolv.ToString() + Environment.NewLine;
                    text += "Номер итерации " + count.ToString() + Environment.NewLine;
                    text += "Тек. время: " + Now.ToString("yyyy.MM.dd-hh:mm:ss-ff") + ". Прошло млсек.: " +
                        (Now - StartTime).TotalMilliseconds.ToString("00000000") + Environment.NewLine;
                    for (byte tempy = 0; tempy < MaxY; tempy++) 
                    {
                        String nextstr = "";
                        for (byte tempx = 0; tempx < MaxX; tempx++)
                        {
                            byte tempn = (byte)(tempy + tempx * MaxY);
                            text += d[tempn].top.ToString() + "|";
                            nextstr += d[tempn].bottom.ToString() + "|";
                                
                        }
                        text += Environment.NewLine + nextstr;
                        text += Environment.NewLine + divline + Environment.NewLine;
                    }
                    text += Environment.NewLine + Environment.NewLine;

                    SetTableText(text);
                    
                    // смещаемся на предыдущую позицию и ищем следущюую позицию домино
                    n--;
                    if (y == 0) // смещаться уже некуда
                    {
                        x--;
                        y = (byte)(MaxY - 1);
                    }
                    else y--;
                    NotNull = true; // в предыдущей позиции стоит костяшка.
                }
            }

            // Отображаем информацию о решении.
            SetTableText(Environment.NewLine + threadName + "Количество решений " + NumSolv.ToString() +
                Environment.NewLine + threadName + "Количество итераций " + count.ToString());
        }

    }

    struct Summs
    {
        byte[] sumX; // суммы вертикальных рядов с координатой x
        byte[] sumY; // суммы горизонтальных рядов с координатой y
        byte UD; // сумма главной диагонали слева на право сверху (Up) вниз (Down)
        byte DU; // сумма главной диагонали слева на право снизу (Down) вверх (Up)
        byte MaxX; // число костяшек по горизонтали
        byte MaxY; // число костяшек по вертикали. Число позиций в таблице чисел по y == MaxY*2
        byte brdX; // == MaxX-1. Для контроля границы номеров рядов от 0 до brdX
        byte brdY; // == MaxY-1. Для контроля границы номеров домино от 0 до brdY
        byte CRC; // контрольная сумма

        // для ускорения расчетов временные переменные
        byte tmpX;
        byte tmpYt;
        byte tmpYb;
        byte tmpDU;
        byte tmpUD;

        public Summs(byte MaxX, byte MaxY, byte CRC)
        {
            this.MaxX = MaxX;
            this.MaxY = MaxY;
            this.CRC = CRC;

            sumX = new byte[MaxX];
            sumY = new byte[MaxY * 2];
            brdX = (byte)(MaxX - 1);
            brdY = (byte)(MaxY - 1);
            UD = 0;
            DU = 0;

            tmpX = 0;
            tmpYt = 0;
            tmpYb = 0;
            tmpDU = 0;
            tmpUD = 0;
        }

        public void Clear()
        {
            for (byte i = 0; i < MaxX; i++) sumX[i] = 0;
            for (byte i = 0; i < MaxY * 2; i++) sumY[i] = 0;
            DU = 0;
            UD = 0;
        }

        // Сумма по вертикали для ряда домино (0...brdX)
        public byte GetX(byte x)
        {
            return sumX[x];
        }
        // Сумма по горизонтали для строки домино (0..MaxY*2-1)
        public byte GetY(byte y)
        {
            return sumY[y];
        }
        public byte GetDU() // Сумма диагонали слева на право, снизу вверх
        {
            return DU;
        }
        public byte GetUD() // // Сумма диагонали слева на право, сверху вниз
        {
            return UD;
        }
        public void SetX(byte x, byte value)
        {
            sumX[x] = value;
        }
        public void SetY(byte y, byte value)
        {
            sumY[y] = value;
        }
        public void SetDU(byte value) // снизу вверх
        {
            DU = value;
        }
        public void SetUD(byte value) // сверху вниз
        {
            UD = value;
        }
        public void AddX(byte x, byte value)
        {
            sumX[x] += value;
        }
        public void AddY(byte y, byte value)
        {
            sumY[y] += value;
        }
        public void AddDU(byte value) // снизу вверх
        {
            DU += value;
        }
        public void AddUD(byte value) // сверху вниз
        {
            UD += value;
        }
        // x 0..MaxX, y 0..MaxY.
        // Выполняет проверку на сумму CRC если установлен флаг check:
        // возвращает код ошибки и не считает дальше. Если проверка не пройдена, то
        // функция удаляет из всех использованных сумм домино d. Таким образом,
        // после вызова функции, которая возвращает код ошибки > 0, можно продолжать проверку,
        // как будто домино d не было учтено в суммах.
        // Коды ошибок:
        // 0. Все проверки успешны.
        // 1. Нет смысла поворачивать костяшку.
        // 2. Можно и нужно повернуть костяшку.
        public byte AddDomino(byte x, byte y, ref Domino d)
        {
            // вначале заполняем вертикальный ряд, он заполняется быстрее, чем строка, поэтому проверяем сумму по вертикали первой. это позволяет нам быстрее отбрасывать ненужные варианты.
            tmpX = (byte)(sumX[x] + d.sum);
            if (y == brdY)
            {
                if (tmpX != CRC) return 1; // нет смысла поворачивать. в любом случае не сойдется.
            }
            else
                if (tmpX > CRC) return 1; // нет смысла поворачивать. в любом случае не сойдется.

            // потом заполняем строку, проверяем ее второй.
            byte yt = (byte)(y + y);
            byte yb = (byte)(yt + 1);

            tmpYt = (byte)(sumY[yt] + d.top);
            if (x == brdX) // строка заполнена
            {
                if (tmpYt != CRC)
                    if (d.IsNotSimmetric)
                        // проверяем сойдется ли сумма верхнего ряда чисел и нижнего, если перевернуть костяшку (экономим на повороте в главном цикле; это занимает больше времени чем проверка здесь)
                        if (sumY[yt] + d.bottom == CRC && sumY[yb] + d.top == CRC) return 2; // повернуть можно
                        else return 1; // повернуть бессмысленно
                    else return 1; // повернуть бессмысленно
            }
            else // строка не заполнена
                if (tmpYt > CRC)
                    if (d.IsNotSimmetric)
                        if (sumY[yt] + d.bottom > CRC || sumY[yb] + d.top > CRC) return 1; // повернуть бессмысленно
                        else return 2; // повернуть можно
                    else return 1; // повернуть бессмысленно

            tmpYb = (byte)(sumY[yb] + d.bottom);
            if (x == brdX)
            {
                if (tmpYb != CRC)
                    if (d.IsNotSimmetric)
                        if (sumY[yt] + d.bottom == CRC && sumY[yb] + d.top == CRC) return 2; // повернуть можно
                        else return 1;
                    else return 1; // повернуть бессмысленно
            }
            else
                if (tmpYb > CRC)
                    if (d.IsNotSimmetric)
                        if (sumY[yt] + d.bottom > CRC || sumY[yb] + d.top > CRC) return 1; // повернуть бессмысленно
                        else return 2; // повернуть можно
                    else return 1; // повернуть бессмысленно

            // диагональ снизу вверх заполняется раньше чем диагональ сверху вниз (она заполниться когда мы поставим
            // костяшку (x=brX;y=0)), поэтому проверяем ее вначале. так можно отсеять быстрее неправильные варианты.
            bool changedDU = false;
            if (x == brdX - yt) // верхняя часть костяшки на DU диагонали
            {
                tmpDU = (byte)(DU + d.top);
                if (y == 0) // вся диагональ DU заполнена
                {
                    if (tmpDU != CRC)
                        if (d.IsNotSimmetric)
                            if (DU + d.bottom != CRC) return 1; // повернуть бессмысленно
                            else return 2; // повернуть можно
                        else return 1; // повернуть бессмысленно
                }
                else
                    if (tmpDU > CRC)
                        if (d.IsNotSimmetric)
                            if (DU + d.bottom > CRC) return 1; // повернуть бессмысленно
                            else return 2; // повернуть можно
                        else return 1; // повернуть бессмысленно

                changedDU = true;
            }
            else if (x == brdX - yb) // нижняя часть костяшки на DU диагонали
            {
                tmpDU = (byte)(DU + d.bottom);
                if (tmpDU > CRC) // здесь никогда не будет заполнена полностью диагональ DU
                    if (d.IsNotSimmetric)
                        if (DU + d.top > CRC) return 1; // повернуть бессмысленно
                        else return 2; // повернуть можно
                    else return 1; // повернуть бессмысленно

                changedDU = true;
            }

            bool changedUD = false;
            if (x == yt) // верхняя часть костяшки на UD диагонали
            {
                tmpUD = (byte)(UD + d.top);
                if (tmpUD > CRC) // никогда не будет здесь заполнена вся диагональ UD
                    if (d.IsNotSimmetric)
                        if (UD + d.bottom > CRC) return 1; // повернуть бессмысленно
                        else return 2; // повернуть можно
                    else return 1; // повернуть бессмысленно

                changedUD = true;
            }
            else if (x == yb) // нижняя часть костяшки на UD диагонали
            {
                tmpUD = (byte)(UD + d.bottom);
                if (x == brdX) // заполнана вся диагональ UD
                {
                    if (tmpUD != CRC)
                        if (d.IsNotSimmetric)
                            if (UD + d.top != CRC) return 1; // повернуть бессмысленно
                            else return 2; // повернуть можно
                        else return 1; // повернуть бессмысленно
                }
                else
                    if (tmpUD > CRC)
                        if (d.IsNotSimmetric)
                            if (UD + d.top > CRC) return 1; // повернуть бессмысленно
                            else return 2; // повернуть можно
                        else return 1; // повернуть бессмысленно

                changedUD = true;

            }

            // Только если прошли все проверки записываем временные данные в суммы
            sumX[x] = tmpX;
            sumY[yt] = tmpYt;
            sumY[yb] = tmpYb;
            if (changedDU) DU = tmpDU;
            if (changedUD) UD = tmpUD;

            return 0;
        }

        // То же что и AddDomino, но без проверки
        public void AddDominoWithoutCheck(byte x, byte y, ref Domino d)
        {
            // вначале заполняем вертикальный ряд, он заполняется быстрее, чем строка, поэтому проверяем сумму по вертикали первой. это позволяет нам быстрее отбрасывать ненужные варианты.
            sumX[x] += d.sum;

            // потом заполняем строку, проверяем ее второй.
            byte yt = (byte)(y * 2);
            byte yb = (byte)(yt + 1);

            sumY[yt] += d.top;
            sumY[yb] += d.bottom;
            // костяшку (x=brX;y=0)), поэтому проверяем ее вначале. так можно отсеять быстрее неправильные варианты.
            if (x == brdX - yt) // верхняя часть костяшки на DU диагонали
                DU += d.top;
            else if (x == brdX - yb) // нижняя часть костяшки на DU диагонали
                DU += d.bottom;

            if (x == yt) // верхняя часть костяшки на UD диагонали
                UD += d.top;
            else if (x == yb) // нижняя часть костяшки на UD диагонали
                UD += d.bottom;

        }

        public void RemoveDomino(byte x, byte y, ref Domino d)
        {
            byte yt = (byte)(y + y); // вертикальная координата верхнего числа костяшки в таблице чисел (не костяшек!)
            byte yb = (byte)(yt + 1); // вертикальная координата нижнего числа костяшки в таблице чисел (не костяшек!)

            sumX[x] -= d.sum; // убираем сумму костяшки из вертикального ряда по х

            sumY[yt] -= d.top; // убираем верхнее число костяшки из верхнего горизонтального ряда чисел
            sumY[yb] -= d.bottom; // убираем нижнее число костяшки из нижнего горизонтального ряда чисел

            // убираем сумму из главных диагоналей
            if (x == yt) UD -= d.top; // верхняя часть костяшки лежит на диагонали сверху вниз
            else if (x == yb) UD -= d.bottom; // нижнаяя часть костяшки лежит на диагонали сверху вниз

            if (x == brdX - yt) DU -= d.top; // верхняя часть костяшки лежит на диагонали снизу вверх
            else if (x == brdX - yb) DU -= d.bottom; // нижняя часть костяшки лежит на диагонали снизу вверх
        }

        public override String ToString()
        {
            String text = "Суммы:" + Environment.NewLine;
            text += "Рядов ";
            for (byte i = 0; i < MaxX; i++) text += sumX[i].ToString("D2") + "|";
            text += Environment.NewLine + "Строк ";
            for (byte i = 0; i < MaxY * 2; i++) text += sumY[i].ToString("D2") + "|";
            text += Environment.NewLine;
            text += "Диагональ снизу вверх " + DU.ToString("D2") + Environment.NewLine;
            text += "Диагональ сверху вниз " + UD.ToString("D2") + Environment.NewLine;

            return text;
        }
    }

}
