using System.Collections.Generic;

public interface IBattleSystem
{
    void ExecuteBattles();
    void Battle(int laneIndex);

    bool CanBattle(int laneIndex);
}
