namespace ConsoleApp17
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Пожалуйста, введите путь до папки:");
            string path = Console.ReadLine(); // читаем путь до папки из консоли
            TimeSpan timeSpan = TimeSpan.FromMinutes(30); // временной интервал в 30 минут

            if (Directory.Exists(path))
            {
                try
                {
                    long initialSize = CalculateDirectorySize(path);
                    Console.WriteLine($"Размер папки до очистки: {initialSize} байт");

                    int filesDeleted = DeleteOldFilesAndFolders(path, timeSpan);
                    Console.WriteLine($"Удалено файлов: {filesDeleted}");

                    long finalSize = CalculateDirectorySize(path);
                    Console.WriteLine($"Размер папки после очистки: {finalSize} байт");

                    long spaceFreed = initialSize - finalSize;
                    Console.WriteLine($"Освобождено места: {spaceFreed} байт");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Папка по заданному пути не существует.");
            }
        }

        static long CalculateDirectorySize(string path)
        {
            long size = 0;
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                size += file.Length;
            }

            foreach (var dir in directory.GetDirectories())
            {
                size += CalculateDirectorySize(dir.FullName); // рекурсивный подсчет размера
            }

            return size;
        }

        static int DeleteOldFilesAndFolders(string path, TimeSpan timeSpan)
        {
            int filesDeleted = 0;
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                if (DateTime.Now - file.LastAccessTime > timeSpan)
                {
                    file.Delete();
                    filesDeleted++;
                }
            }

            foreach (var dir in directory.GetDirectories())
            {
                DateTime lastWriteTime = dir.LastWriteTime; // сохраняем время последнего изменения подпапки
                filesDeleted += DeleteOldFilesAndFolders(dir.FullName, timeSpan); // рекурсивное удаление

                // Если подпапка пуста после удаления старых файлов и подпапок, удаляем ее
                if (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0)
                {
                    if (DateTime.Now - lastWriteTime > timeSpan) // используем сохраненное время последнего изменения
                    {
                        dir.Delete();
                    }
                }
            }

            return filesDeleted;
        }
    }
}
