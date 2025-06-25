namespace ToDoList
{
    public class Test_semgrep
    {
        public int SomeRiskyOperation()
        {
            return 0;
        }
        public void errors()
        {
            string apiKey = "test_1234567890";

            string userName = "admin";

            var query = "SELECT * FROM Users WHERE Name = '" + userName + "'";



            try
            {
                SomeRiskyOperation();
            }
            catch (Exception)  
            {
            }

        }


    }
}
