namespace Sports.Results
{
    public class Team
    {
        private readonly string _name;
        private readonly string _score;
        public Team(string name, string score)
        {
            _name = name;
            _score = score;
        }
        public override string ToString()
        {
            return $"{_name} {_score}";
        }
    }
}