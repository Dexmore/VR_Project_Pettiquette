using UnityEngine;

public interface IAnimalState
{
    public void EnterState(AnimalLogic animal);
    public void UpdateState();
    public void ExitState();
}
