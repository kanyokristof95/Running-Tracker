namespace Running_Tracker.Persistence
{
    public class PersonalDatas
    {
        public Gender Sex { get; set; }

        public int Height { get; set; }

        public int Weight { get; set; }

        public PersonalDatas(Gender sex = Gender.Male, int height = 170, int weight = 70)
        {
            Sex = sex;
            Height = height;
            Weight = weight;
        }
    }
}