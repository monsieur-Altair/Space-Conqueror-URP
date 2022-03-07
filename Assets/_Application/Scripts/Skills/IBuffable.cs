namespace _Application.Scripts.Skills
{
    public interface IBuffable
    {
        void Buff(float percent);
        void UnBuff(float percent);
    }
}