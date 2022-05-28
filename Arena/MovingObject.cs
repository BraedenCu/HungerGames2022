namespace Arena
{
    abstract public class MovingObject : ArenaObject
    {
        public override bool IsUpdating => true;

        private Turn turn = null;

        public MovingObject(int graphicCode, int layer, double width, double height)
            : base(graphicCode, layer, width, height)
        { }

        abstract protected void UserDefinedBeginningOfTurn();
        abstract protected void UserDefinedEndOfTurn();
        abstract protected Turn UserDefinedChooseAction();

        public void BeginningOfTurn()
        {
            UserDefinedBeginningOfTurn();
        }

        public bool ChooseAction()
        {
            turn = UserDefinedChooseAction();
            return true;
        }

        public void ExecuteAction()
        {
            DoTurn(turn);
        }

        abstract protected bool DoTurn(Turn turn);

        public void EndOfTurn()
        {
            UserDefinedEndOfTurn();
        }
    }
}
