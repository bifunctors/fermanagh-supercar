#pragma warning disable CS8321
#pragma warning disable CS0219

string STAFF_CSV = "staff.csv";
string CUSTOMERS_CSV = "customers.csv";
string QUESTIONS_CSV = "questions.csv";
string REACTION_CSV = "reaction_time_results.csv";
string LAP_TIME_CSV = "lap_times.csv";

string TABLE_DIVIDER = " | ";


Random rng = new Random();

Main();

void Main() {
    // Login();
    while(true) {
        MainMenu();
    }
}

void Login() {
    string[,] staff_details = ReadCsv(STAFF_CSV);

    Console.Clear();
    ReadOnlySpan<char> username = Prompt("Please Enter Your Username");
    ReadOnlySpan<char> password = Prompt("Please Enter Your Password");

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
    string banner = MainMenuBanner();

    string[] menu_options = { "Record Customer Details", "Take Quiz", "Take Reaction Test", "Lap Times", "Report" };

    int selected_menu_option = PickOption(banner, menu_options);

    ShowMenu((MenuOption)selected_menu_option);
}

void ShowMenu(MenuOption option) {
    switch(option) {
        case MenuOption.RECORD_DETAILS:
            NewCustomer();
            break;
        case MenuOption.QUIZ:
            Quiz();
            break;
        case MenuOption.REACTION_TEST:
            ReactionTest();
            break;
        case MenuOption.LAP_TIME:
            LapTime();
            break;
        case MenuOption.REPORTS:
            Report();
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

    Console.Clear();
    Notify("Created New Customer.");

    string first_name_sanitised = new string(first_name.ToArray().Where(x => char.IsAsciiLetterOrDigit(x)).ToArray());
    string surname_sanitised = new string(surname.ToArray().Where(x => char.IsAsciiLetterOrDigit(x)).ToArray());
    string DOB_sanitised = new string(DOB.ToArray().Where(x => char.IsAsciiLetterOrDigit(x) || x == '/').ToArray());
    string gender_sanitised = new string(gender.ToArray().Where(x => char.IsAsciiLetterOrDigit(x)).ToArray());

    string number = rng.Next().ToString();
    while(true) {
        if(number.Length > 3) break;
        number = rng.Next().ToString();
    }

    string username = $"{first_name_sanitised[0]}{surname_sanitised}{number[..3].ToString()}";

    AppendCsv(CUSTOMERS_CSV, $"{username},{first_name_sanitised},{surname_sanitised},{DOB_sanitised},{gender_sanitised},Learner");
}

int PickOption(string prompt, string[] options)  {
    int idx = 0;
    while(true) {
        Console.Clear();

        ColourBlue();
        Console.WriteLine(prompt);
        Console.WriteLine();
        ColourWhite();

        if(idx < 0) idx = options.Length - 1;
        if(idx >= options.Length) idx = 0;

        for(int i = 0; i < options.Length; i++) {
            ColourGreen();
            Console.Write(idx == i ? ">" : " ");
            ColourWhite();
            Console.WriteLine($" {options[i]}");
        }

        ConsoleKeyInfo key_info = Console.ReadKey(false);
        if(key_info.Key == ConsoleKey.DownArrow || key_info.Key == ConsoleKey.J) idx++;
        if(key_info.Key == ConsoleKey.UpArrow || key_info.Key == ConsoleKey.K) idx--;
        if(key_info.Key == ConsoleKey.Enter || key_info.Key == ConsoleKey.L) return idx;
    }
}

void Quiz() {
    string banner = QuizBanner();

    string[,] customer_details = ReadCsv(CUSTOMERS_CSV);

    string[] customer_details_flat = Enumerable.Range(0, customer_details.GetLength(0))
            .Select(i => $"{customer_details[i,0]}, {customer_details[i,1]} {customer_details[i,2]}").ToArray();

    int selected_customer_index = PickOption(banner, customer_details_flat);

    string[,] quiz_details = ReadCsv(QUESTIONS_CSV);

    int score = 0;

    int[] answers = new int[2];

    for(int i = 0; i < quiz_details.GetLength(0); i++) {
        string[] quiz_questions = [quiz_details[i,1], quiz_details[i,2], quiz_details[i,3]];
        int question_answer_index = PickOption($"Question: {quiz_details[i,0]}", quiz_questions);

        if(question_answer_index == int.Parse(quiz_details[i,4]) - 1) score++;

        answers[i] = question_answer_index;
    }

    // Store in CSV
    Console.WriteLine();

    AppendCsv("quiz_results.csv", $"{customer_details[selected_customer_index, 0]},{score}");

    Console.Clear();
    Notify($"Quiz Completed. You scored {score} out of {quiz_details.GetLength(0)}");
}

void ReactionTest() {
    string banner = ReactionTestBanner();

    string[,] customer_details = ReadCsv(CUSTOMERS_CSV);

    string[] customer_details_flat = Enumerable.Range(0, customer_details.GetLength(0))
            .Select(i => $"{customer_details[i,0]}, {customer_details[i,1]} {customer_details[i,2]}").ToArray();

    int selected_customer_index = PickOption(banner, customer_details_flat);

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch reaction_stopwatch = new System.Diagnostics.Stopwatch();
    bool reaction_timer_started = false;

    int time = rng.Next(2, 5);
    int reaction_time = -1;

    stopwatch.Start();

    Console.Clear();
    Console.WriteLine("Wait Until Green!");

    while(true) {
        if(reaction_timer_started) {
            Console.Clear();
            ColourGreen();
            Console.WriteLine("Click Space!");
            ColourWhite();
            Console.ReadKey(false);
            break;
        }

        if(stopwatch.Elapsed.Seconds > time) {
            reaction_timer_started = true;
            reaction_stopwatch.Start();
        }
    }

    double elapsed_reaction_time = reaction_stopwatch.Elapsed.Milliseconds;

    AppendCsv(REACTION_CSV, $"{customer_details[selected_customer_index, 0]},{elapsed_reaction_time.ToString()}");

    Console.Clear();
    Notify($"You took {elapsed_reaction_time} ms to click space.");
}

void LapTime() {
    string banner = LapTimeBanner();

    string[,] customer_details = ReadCsv(CUSTOMERS_CSV);

    string[] customer_details_flat = Enumerable.Range(0, customer_details.GetLength(0))
            .Select(i => $"{customer_details[i,0]}, {customer_details[i,1]} {customer_details[i,2]}").ToArray();

    int selected_customer_index = PickOption(banner, customer_details_flat);

    System.Text.StringBuilder lap_time_string_builder = new();

    for(int i = 0; i < 5; i++) {
        Console.WriteLine();
        string lap_time_entered = Prompt($"Please Enter Lap Time {i+1}").ToString();

        if(!double.TryParse(lap_time_entered, out double lap_time)) {
            Notify("A valid number was not entered. Please Try Again.");
            i--;
            continue;
        }

        lap_time_string_builder.Append(",");
        lap_time_string_builder.Append(lap_time.ToString());
    }

    AppendCsv(LAP_TIME_CSV, $"{customer_details[selected_customer_index, 0]}{lap_time_string_builder.ToString()}");

    Notify("All lap times have been entered.");
}

void Report() {
    string banner = ReportsBanner();

    string[] report_types = ["Overall", "Customer Specific"];

    int selected_report_type_index = PickOption(banner, report_types);

    switch(selected_report_type_index) {
        case 0:
            ReportOverall();
            break;
        case 1:
            ReportSpecific();
            break;
    }

}


void ReportOverall() {
    string banner = ReportsBanner();

    string[,] customer_details = ReadCsv(CUSTOMERS_CSV);

    string[] customer_details_flat = Enumerable.Range(0, customer_details.GetLength(0))
            .Select(i => $"{customer_details[i,0]}, {customer_details[i,1]} {customer_details[i,2]}").ToArray();

    string[,] lap_times = ReadCsv(LAP_TIME_CSV);

    string[,] customer_report_information = new string[customer_details.GetLength(0), 2];

    for(int i = 0; i < customer_details.GetLength(0); i++) {
        // get lap times for customer
        string[][] user_lap_times = GetAllRowsForUser(lap_times, customer_details[i, 0]);

        double[] user_lap_times_flattened = Enumerable.Range(0, user_lap_times.Length)
            .Select(i => new double[] {
                    StringToDouble(user_lap_times[i][1]),
                    StringToDouble(user_lap_times[i][2]),
                    StringToDouble(user_lap_times[i][3]),
                    StringToDouble(user_lap_times[i][4]),
                    StringToDouble(user_lap_times[i][5])})
            .Select(i => i.Average())
            .ToArray();

        Console.WriteLine("Lap times length flattened: " + user_lap_times_flattened.Length);

        Console.WriteLine($"User lap time: {user_lap_times_flattened[0]}");

        double user_lap_time_average = user_lap_times_flattened.Length == 0 ? -1 : user_lap_times_flattened.Average();

        customer_report_information[i, 0] = customer_details[i, 0];
        customer_report_information[i, 1] = user_lap_time_average.ToString();
    }

    Console.ReadKey();

    Console.Clear();

    int username_length = "Username".Length;
    int lap_average_length = "Lap Average".Length;

    int[] minimum_widths = [username_length, lap_average_length];


    // Set widths for each column
    for(int i = 0; i < customer_report_information.GetLength(0); i++) {
        for(int j = 0; j < customer_report_information.GetLength(1); j++) {
            if(customer_report_information[i,j].Length > minimum_widths[j])
                minimum_widths[j] = customer_report_information[i,j].Length;
        }
    }

    int total_width = minimum_widths.Sum() + (TABLE_DIVIDER.Length * minimum_widths.Length);

    for(int j = 0; j < total_width; j++) {
        if(j == 0 || j == total_width - 1)
            Console.Write('+');
        else
            Console.Write('-');
    }
    Console.WriteLine();

    Console.WriteLine($"|{"Username".PadRight(minimum_widths[0])} | {"Lap Average".PadRight(minimum_widths[1])} |");

    for(int j = 0; j < total_width; j++) {
        if(j == 0 || j == total_width - 1)
            Console.Write('+');
        else
            Console.Write('-');
    }
    Console.WriteLine();

    for(int i = 0; i < customer_report_information.GetLength(0); i++) {
        Console.Write("|");
        for(int j = 0; j < customer_report_information.GetLength(1); j++) {
            string grid_info = customer_report_information[i,j];
            Console.Write($"{grid_info.PadRight(minimum_widths[j])} | ");
        }

        Console.WriteLine();

        for(int j = 0; j < total_width; j++) {
            if(j == 0 || j == total_width - 1)
                Console.Write('+');
            else
                Console.Write('-');
        }

        Console.WriteLine();
    }


    Console.WriteLine();
    Console.WriteLine("Press any key to return to the main menu");
    Console.ReadKey(false);

}

double StringToDouble(string str) {
    return Convert.ToDouble(str);
}

int MaximumLength(string[] array) {
    return array.Max(x => x.Length);
}

string[][] GetAllRowsForUser(string[,] array, string username) {
    List<string[]> valid_rows = new();

    for(int i = 0; i < array.GetLength(0); i++) {
        if(array[i,0] != username) continue;

        string[] row_arr = new string[array.GetLength(1)];

        for(int j = 0; j < array.GetLength(1); j++) {
            row_arr[j] = array[i, j];
        }

        valid_rows.Add(row_arr);
    }

    return valid_rows.ToArray();
}

double AverageLapArray(double[] lap_times) {
    double running_total = 0;
    for(int i = 0; i < lap_times.Length; i++) {
        running_total += lap_times[i];
    }
    return running_total / lap_times.Length;
}

void ReportSpecific() {
    string banner = ReportsBanner();

    string[,] customer_details = ReadCsv(CUSTOMERS_CSV);

    string[] customer_details_flat = Enumerable.Range(0, customer_details.GetLength(0))
            .Select(i => $"{customer_details[i,0]}, {customer_details[i,1]} {customer_details[i,2]}").ToArray();

    int selected_customer_index = PickOption(banner, customer_details_flat);

    Console.Clear();
}

ReadOnlySpan<char> Prompt(string prompt) {
    Console.WriteLine($"{prompt}");
    Console.Write($"> ");
    return Console.ReadLine().AsSpan()!;
}

void Notify(string message) {
    Console.WriteLine($"{message}");
    Console.WriteLine($"Press Any Key To Continue");
    Console.ReadKey(false);
}

void ColourRed() => Console.ForegroundColor = ConsoleColor.Red;
void ColourGreen() => Console.ForegroundColor = ConsoleColor.Green;
void ColourBlue() => Console.ForegroundColor = ConsoleColor.Blue;
void ColourWhite() => Console.ForegroundColor = ConsoleColor.White;
void ColourGray() => Console.ForegroundColor = ConsoleColor.Gray;

string MainMenuBanner() {
    string[] lines = {
        @" __  __       _         __  __",
        @"|  \/  | __ _(_)_ __   |  \/  | ___ _ __  _   _",
        @"| |\/| |/ _` | | '_ \  | |\/| |/ _ \ '_ \| | | |",
        @"| |  | | (_| | | | | | | |  | |  __/ | | | |_| |",
        @"|_|  |_|\__,_|_|_| |_| |_|  |_|\___|_| |_|\__,_|"
    };

    return string.Join(Environment.NewLine, lines);
}

string QuizBanner() {
    string[] lines = {
        @"  ___        _",
        @" / _ \ _   _(_)____",
        @"| | | | | | | |_  /",
        @"| |_| | |_| | |/ /",
        @" \__\_\\__,_|_/___|"
    };

    return string.Join(Environment.NewLine, lines);
}

string ReactionTestBanner() {
    string[] lines = {
        @" ____                 _   _               _____         _",
        @"|  _ \ ___  __ _  ___| |_(_) ___  _ __   |_   _|__  ___| |_",
        @"| |_) / _ \/ _` |/ __| __| |/ _ \| '_ \    | |/ _ \/ __| __|",
        @"|  _ <  __/ (_| | (__| |_| | (_) | | | |   | |  __/\__ \ |_",
        @"|_| \_\___|\__,_|\___|\__|_|\___/|_| |_|   |_|\___||___/\__|"
    };

    return string.Join(Environment.NewLine, lines);
}

string LapTimeBanner() {
    string[] lines = {
        @" _                  _____ _",
        @"| |    __ _ _ __   |_   _(_)_ __ ___   ___  ___",
        @"| |   / _` | '_ \    | | | | '_ ` _ \ / _ \/ __|",
        @"| |__| (_| | |_) |   | | | | | | | | |  __/\__ \",
        @"|_____\__,_| .__/    |_| |_|_| |_| |_|\___||___/",
        @"           |_|"
    };

    return string.Join(Environment.NewLine, lines);
}

string ReportsBanner() {
    string[] lines = {
        @" ____                       _",
        @"|  _ \ ___ _ __   ___  _ __| |_ ___",
        @"| |_) / _ \ '_ \ / _ \| '__| __/ __|",
        @"|  _ <  __/ |_) | (_) | |  | |_\__ \",
        @"|_| \_\___| .__/ \___/|_|   \__|___/",
        @"          |_|"
    };

    return string.Join(Environment.NewLine, lines);
}

enum MenuOption {
    RECORD_DETAILS = 0,
    QUIZ = 1,
    REACTION_TEST = 2,
    LAP_TIME = 3,
    REPORTS = 4
}

enum ReportType {
    OVERALL = 0,
    SPECIFIC = 1
}
