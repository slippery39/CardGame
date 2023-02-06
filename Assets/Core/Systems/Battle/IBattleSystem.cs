using System.Collections.Generic;

public interface IBattleSystem
{
    void Battle(int laneIndex);

    bool CanBattle(int laneIndex);
}
