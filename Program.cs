#pragma warning disable CS8321
#pragma warning disable CS0219

string STAFF_CSV = "staff.csv";
string CUSTOMERS_CSV = "customers.csv";
bool LOGGED_IN = true;

Main();

void Main() {
    MainMenu();
}

void Login() {
    string[,] staff_details = ReadCsv(STAFF_CSV);

    Console.Clear();
    ReadOnlySpan<char> username = Prompt("Please Enter Your Username");
    ReadOnlySpan<char> password = Prompt("Please Enter Your Password");

    for(int i = 0; i < staff_details.GetLength(0); i++) {
        if(staff_details[i, 0] == username && staff_details[i, 1] == password) {
            LOGGED_IN = true;
            return;
        }
    }

    ColourRed();
    Notify("Incorrect Login Details");
    ColourWhite();
    Login();
}

void MainMenu() {
    int minimum_option_index = LOGGED_IN ? 1 : 0;
    int selected_option_index = minimum_option_index;
    string[] menu_options = { "Login", "Record Customer Details", "Take Quiz", "Lap Times", "Report" };

    // Login
    // Options ( Logged In )
    // Record Customer Details ( Logged In )
    // Quiz ( Logged In )
    // Reaction Test ( Logged In )
    // Allowed Category ( Logged In )

    while(true) {
        if(LOGGED_IN) minimum_option_index = 1;

        Console.Clear();
        Banner("Main Menu");
        Console.WriteLine();

        if(selected_option_index < minimum_option_index) selected_option_index = menu_options.Length - 1;
        if(selected_option_index >= menu_options.Length) selected_option_index = 0;

        for(int i = minimum_option_index; i < menu_options.Length; i++) {
            if(!LOGGED_IN && i != 0) ColourGray();
            Console.WriteLine($"{(selected_option_index == i ? "> " : "")}{menu_options[i]}");
            ColourWhite();
        }

        ConsoleKeyInfo key = Console.ReadKey();
        if(key.Key == ConsoleKey.J) selected_option_index++;
        if(key.Key == ConsoleKey.K) selected_option_index--;

        if(key.Key == ConsoleKey.L) {
            ShowMenu((MenuOption)selected_option_index);
        }
    }
}

void ShowMenu(MenuOption option) {
    switch(option) {
        case MenuOption.LOGIN:
            Login();
            break;
        case MenuOption.RECORD_DETAILS:
            if(!LOGGED_IN) break;
            NewCustomer();
            break;
        case MenuOption.QUIZ:
            if(!LOGGED_IN) break;
            break;
        case MenuOption.LAP_TIME:
            if(!LOGGED_IN) break;
            break;
        case MenuOption.REPORTS:
            if(!LOGGED_IN) break;
            break;
    }
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

void WriteCsv(string path, string[,] data) {
    File.WriteAllText(path, "");
    foreach(var line in data) {
        string joined = string.Join(",", line);
        File.AppendAllText(path, joined);
    }
}

void AppendCsv(string path, string data) {
    File.AppendAllText(path, data);
    File.AppendAllText(path, "\n");
}

void NewCustomer() {
    Console.Clear();
    ReadOnlySpan<char> first_name = Prompt("What is the Customers' Forename?");
    ReadOnlySpan<char> surname = Prompt("What is the Customers' Surname?");
    ReadOnlySpan<char> DOB = Prompt("What is the Customers' DOB?");
    ReadOnlySpan<char> gender = Prompt("What is the Customers' Gender?");

    bool valid_date = DateTime.TryParse(DOB, out DateTime _);

    if(!valid_date) {
        ColourRed();
        Notify("Invalid Date Was Input.");
        ColourWhite();
        NewCustomer();
        return;
    }

    // Sanitise Here
    Notify("Created New Customer.");

    string first_name_sanitised = new string(first_name.ToArray().Where(x => char.IsAsciiLetterOrDigit(x)).ToArray());
    string surname_sanitised = new string(surname.ToArray().Where(x => char.IsAsciiLetterOrDigit(x)).ToArray());
    string DOB_sanitised = new string(DOB.ToArray().Where(x => char.IsAsciiLetterOrDigit(x) || x == '/').ToArray());
    string gender_sanitised = new string(gender.ToArray().Where(x => char.IsAsciiLetterOrDigit(x)).ToArray());

    AppendCsv(CUSTOMERS_CSV, $"{first_name_sanitised},{surname_sanitised},{DOB_sanitised},{gender_sanitised}");
}

ReadOnlySpan<char> Prompt(string prompt) {
    Console.WriteLine($"{prompt}");
    Console.Write($"> ");
    return Console.ReadLine().AsSpan()!;
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
void ColourGray() => Console.ForegroundColor = ConsoleColor.Gray;

void Banner(string message) {
    Console.WriteLine("---------------------");
    Console.WriteLine($"{message}");
    Console.WriteLine("---------------------");
}

enum MenuOption {
    LOGIN = 0,
    RECORD_DETAILS = 1,
    QUIZ = 2,
    REACTION_TEST = 3,
    LAP_TIME = 4,
    REPORTS = 5
}
