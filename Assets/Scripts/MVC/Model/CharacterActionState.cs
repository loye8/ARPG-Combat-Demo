namespace ARPGCombat.MVC.Model { 
    public enum CharacterActionState
    {
        Idle,
        Moving,
        Attacking,
        Hurt,
        Dead
    }

    public static class CharacterStateMatrix
    {
        public static bool CanTransition(CharacterActionState from,CharacterActionState to)
        {
            if(from == CharacterActionState.Dead) return false;
            if(to == CharacterActionState.Dead) return true;
            if(from == CharacterActionState.Hurt)
                return to == CharacterActionState.Idle;
            if(from == CharacterActionState.Attacking)
                return to == CharacterActionState.Hurt || to == CharacterActionState.Idle || to  == CharacterActionState.Dead;
            return true;
        }
    }
}