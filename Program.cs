string STAFF_CSV = "staff.csv";

Main();

void Main() {
    Login();
    MainMenu();
}

void Login() {
    string[,] staff_details = ReadCsv(STAFF_CSV);

    Console.Clear();
    string username = Prompt("Please Enter Your Username");
    string password = Prompt("Please Enter Your Password");

    for(int i = 0; i < staff_details.GetLength(0); i++) {
        if(staff_details[i, 0] == username && staff_details[i, 1] == password) {
            return;
        }
    }

    ColourRed();
    Notify("Incorrect Login Details");
    ColourWhite();
    Login();
}

void MainMenu() {
    Console.Clear();
    Banner("Main Menu");

    // Login
    // Options ( Logged In )
    // Record Customer Details ( Logged In )
    // Quiz ( Logged In )
    // Reaction Test ( Logged In )
    // Allowed Category ( Logged In )

    while(true) {}
}

string[,] ReadCsv(string path) {
    string[] file_lines = File.ReadAllLines(path);

    int cols = file_lines[0].Split(',').Length;

    // Rows, Cols
    string[,] csv_data = new string[file_lines.Length, cols];

    for(int i = 0; i < file_lines.Length; i++) {
        string row = file_lines[i];
        string[] col = row.Split(',');

        for(int j = 0; j < col.Length; j++) {
            csv_data[i, j] = col[j];
        }
    }

    return csv_data;
}

string Prompt(string prompt) {
    Console.WriteLine($"{prompt}");
    Console.Write($"> ");
    return Console.ReadLine()!;
}

void Notify(string message) {
    Console.WriteLine($"{message}");
    Console.WriteLine($"Press Any Key To Continue");
    Console.ReadKey();
}

void ColourRed() => Console.ForegroundColor = ConsoleColor.Red;
void ColourGreen() => Console.ForegroundColor = ConsoleColor.Green;
void ColourBlue() => Console.ForegroundColor = ConsoleColor.Blue;
void ColourWhite() => Console.ForegroundColor = ConsoleColor.White;

void Banner(string message) {
    Console.WriteLine("---------------------");
    Console.WriteLine($"{message}");
    Console.WriteLine("---------------------");
}
