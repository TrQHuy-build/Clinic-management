namespace QL_Nha_Khoa
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            using (var login = new LoginForm())
            {
                var result = login.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Open the appropriate main form based on role (normalize whitespace/casing)
                    var role = CurrentUser.Instance.Role?.Trim().ToLowerInvariant();
                    switch (role)
                    {
                        case "admin":
                            Application.Run(new AdminForm());
                            break;
                        case "staff":
                            Application.Run(new StaffForm());
                            break;
                        case "doctor":
                            Application.Run(new DoctorForm());
                            break;
                        default:
                            Application.Run(new Form1());
                            break;
                    }
                }
            }
        }
    }
}