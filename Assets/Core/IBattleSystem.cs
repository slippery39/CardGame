using System.Collections.Generic;

public interface IBattleSystem
{
    void ExecuteBattles(List<CardInstance> lane1, List<CardInstance> lane2);
}
