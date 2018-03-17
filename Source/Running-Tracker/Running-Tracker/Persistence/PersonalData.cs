namespace Running_Tracker.Persistence
{
    /// <summary>
    /// Contains the user's personal datas.
    /// </summary>
    public class PersonalData
    {
        /// <summary>
        /// Sex of user.
        /// </summary>
        public Gender Sex { get; set; }

        /// <summary>
        /// Height of user in meter.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Weight of user in kg.
        /// </summary>
        public int Weight { get; set; }

        public PersonalData(Gender sex = Gender.Male, int height = 170, int weight = 70)
        {
            Sex = sex;
            Height = height;
            Weight = weight;
        }
    }
}