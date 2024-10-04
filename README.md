# Физика. ДЗ 4 лекция

## Ссылка на исходный код:  [тык](https://github.com/hizu77/Phys4lec)

## Ссылка на скрины и видео работающей программы: [тык](https://drive.google.com/drive/folders/1Opu6lIBbRlICiluNE13NM3d4d88X0DIT?usp=drive_link)

P.S Мне не удалось загрузить файлы exe(к тому же чтоб запустить их, необходимо будет скачать .NET Framework и тд. Поэтому я решил загрузить видео и скрины, а также описание).

# Код написан на: C# .NET Framework 4.8.1

# Описание основных методов

1. **Запуск программы.**
    
    Аргументы подаются в командную строку в формате:
    
    $[xSize,ySize,xVelocity1,yVelocity1, radius1,mass1, xVelocity2, yVelocity2,radius2,mass2]$**Структура Main:**
    
    ```csharp
    static void Main(string[] args)
            {
    	          var arguments = args.Select(float.Parse).ToArray();
                
                var size = new float[2];
                Array.Copy(arguments, 0, size, 0, 2);
    
                var firstBall = new float[4];
                Array.Copy(arguments, 2, firstBall, 0, 4);
                
                var secondBall = new float[4];
                Array.Copy(arguments, 6, secondBall, 0, 4);
                
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(firstBall, secondBall, size));
            }
    ```
    
    Тут происходит парсинг и группировка аргументов. А также запуск визуализации.
    
2. **class Ball(класс представляющий один шарик)**
    1. Атрибуты класса: Текущие координаты, скорость по каждой из осей, радиус и масса.
        
        ```csharp
               public float X { get; private set; }
               public float Y { get; private set; }
               public float Radius { get;}
               public float VX { get; private set; } // X velocity
               public float VY { get; private set; } // Y velocity
               public float Mass { get;}
        ```
        
    2. Проверка на столкновение с стенками: координаты границы шарика сравниваются с координатами стенок. Если мы столкнулись c стенкой, то меняем направление скорости по одной из осей на противоположное.
        
        ```csharp
        public void CheckWallCollision(Size clientSize)
                {
                    // Проверка на левую и правую стенку
                    if (X - Radius < 0 || X + Radius > clientSize.Width)
                    {
                        VX = -VX;
                    }
        
                    // Проверка на нижнюю и верхнюю стенку
                    if (Y - Radius < 0 || Y + Radius > clientSize.Height)
                    {
                        VY = -VY;
                    }
                }
        ```
        
    3. Проверка на столкновение с другим шариком: для этого сравниваем расстояние между их центрами с суммой их радиусов. Если это расстояние меньше суммы, то шарики столкнулись.
        
        ```csharp
        public bool CheckCollision(Ball other)
                {
                    float dx = other.X - X;
                    float dy = other.Y - Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
        
                    return distance < (Radius + other.Radius);
                }
        ```
        
    4. Разрешение столкновения между шарами:
        
        Чтобы определить новые скорости двух шаров по осям x и y после **абсолютно упругого нецентрального столкновения**, нужно использовать закон сохранения импульса и закон сохранения энергии. Но перед этим необходимо разделить скорости шаров на две компоненты:
        
        1. **Нормальная компонента** — вдоль линии, соединяющей центры шаров.
        2. **Касательная компонента** — перпендикулярная нормальной компоненте. Она не меняется при абсолютно упругом столкновении, так как силы действуют только вдоль линии центров.
        
        Нормальный единичный вектор $\hat n$, направленный вдоль линии столкновения:
        
        $\hat n = \left( \frac{dx}{d}, \frac{dy}{d} \right)$
        
        где $dx=x2−x1, \ dy=y2−y1 \  и \ d= \sqrt{dx^2 + dy^2}$ — расстояние между центрами тел.
        
        Касательный единичный вектор $\hat t$, перпендикулярный нормальному:
        
        $\hat t = \left( -\frac{dy}{d}, \frac{dx}{d} \right)$
        
        Теперь можно разложить скорости на нормальные и касательные компоненты.
        
        - Проекция скорости тела 1 на нормальный вектор:$v_{1n}=\vec v_1⋅\hat n$
        - Проекция скорости тела 1 на касательный вектор:$v_{1t}=\vec v_1⋅\hat t$
        
        Аналогично для второго тела.
        
        Применим ЗСИ и ЗСЭ для вычисления новых нормальных компонент скоростей и получим:
        
        $v'_{1n}=\frac{(m_1-m_2)v_{1n}+2m_2v_{2n}}{m_1+m_2}$
        
        $v'_{2n}=\frac{(m_2-m_1)v_{2n}+2m_1v_{1n}}{m_1+m_2}$
        
        После этого перейдем к старой СК и вычислим новые скорости по осям X Y 
        
        ```csharp
        public static void ResolveCollision(Ball b1, Ball b2)
                {
                    float dx = b2.X - b1.X;
                    float dy = b2.Y - b1.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
        
                    if (distance == 0) return;
        
                    float nx = dx / distance;
                    float ny = dy / distance;
                    
                    float v1n = b1.VX * nx + b1.VY * ny;
                    float v2n = b2.VX * nx + b2.VY * ny;
                    
                    float v1t = - b1.VX * ny + b1.VY * nx;
                    float v2t = - b2.VX * ny + b2.VY * nx;
                    
                    
                    float rV1n = ((b1.Mass - b2.Mass) * v1n + 2 * b2.Mass * v2n) / (b1.Mass + b2.Mass);
                    float rV2n = ((b2.Mass - b1.Mass) * v2n + 2 * b1.Mass * v1n) / (b1.Mass + b2.Mass);
                    
                    float v1x = rV1n * nx - v1t * ny;
                    float v1y = rV1n * ny + v1t * nx;
                    
                    
                    float v2x = rV2n * nx - v2t * ny;
                    float v2y = rV2n * ny + v2t * nx;
        
                    b1.VX = v1x;
                    b2.VX = v2x;
                    
                    b1.VY = v1y;
                    b2.VY = v2y;
                }
        ```
        
3. **class Form1(класс визуализации)**
    1. Обновление картинки(сама визуализация): Тут все просто - двигаем шарики, проверяем столкновения, двигаем шарики, проверяем столкновение и так далее..
        
        ```csharp
        private void UpdateSimulation(object sender, EventArgs e)
                {
                    // Двигаем шарики
                    _ball1.Move();
                    _ball2.Move();
        
                    // Проверям столкновение с стенками
                    _ball1.CheckWallCollision(ClientSize);
                    _ball2.CheckWallCollision(ClientSize);
        
                    // Проверям столкновение шариков
                    if (_ball1.CheckCollision(_ball2))
                    {
                        Ball.ResolveCollision(_ball1, _ball2);
                    }
        
                    Invalidate(); // Перерисовываем
                }
        ```