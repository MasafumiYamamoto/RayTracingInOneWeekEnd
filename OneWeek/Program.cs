namespace OneWeek
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const int imageWidth = 256;
            const int imageHeight = 256;
            char[] bars = {'/', '-', '\\', '|'};

            const string fileName = "./hoge.ppm";
            var content = string.Empty;
            content += $"P3\n{imageWidth.ToString()} {imageHeight.ToString()}\n255\n";

            for (var j = imageHeight - 1; j >= 0; j--)
            {
                Console.Write(bars[j % 4]);
                Console.Write($"{((int) 100.0 * (imageHeight - j) / imageHeight).ToString()}%");
                Console.SetCursorPosition(0, Console.CursorTop);
                
                for (var i = 0; i < imageWidth; i++)
                {
                    var r = (float) i / (imageWidth - 1);
                    var g = (float) j / (imageWidth - 1);
                    const float b = 0.5f;

                    var ir = (int) (255.999 * r);
                    var ig = (int) (255.999 * g);
                    const int ib = (int) (255.999 * b);

                    content += $"{ir} {ig} {ib} \n";
                }
            }

            File.WriteAllText(fileName, content);

        }
    }
}