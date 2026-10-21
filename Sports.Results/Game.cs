namespace Sports.Results
{
    public class Game
    {
        private readonly string _dateTime;
        private readonly Team _winner;
        private readonly Team _loser;
        private readonly string _status;

        public Game(string dateTime, Team winner, Team loser, string status)
        {
            _dateTime = dateTime; 
            _winner = winner; 
            _loser = loser; 
            _status = status;
        }

        public override string ToString()
        {
            return $"{_loser} - {_winner} | {_status}";
        }
    }
}