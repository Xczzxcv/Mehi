using UnityEngine;

public class TurnsManager
{
    public struct Config
    { }

    public enum TurnPhase
    {
        PlayerMove,
        AIMove,
    }

    public int TurnIndex { get; private set; }
    public TurnPhase Phase { get; private set; }

    public const TurnPhase INIT_PHASE = TurnPhase.PlayerMove;
    public const TurnPhase END_PHASE = TurnPhase.AIMove;

    private readonly Config _config;

    public TurnsManager(Config config)
    {
        _config = config;
    }

    public void NextPhase()
    {
        if (Phase == INIT_PHASE)
        {
            Phase = END_PHASE;
        }
        else if (Phase == END_PHASE)
        {
            TurnIndex++;
            Phase = INIT_PHASE;
        }

        Debug.Log($"[{nameof(TurnsManager)}, {nameof(NextPhase)}] TI: {TurnIndex} PH: {Phase}");
        GlobalEventManager.Turns.TurnUpdated.HappenedWith(TurnIndex, Phase);
    }
}