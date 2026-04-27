/// <summary>
/// All high-level states the training simulation can be in.
/// One state corresponds roughly to one Unity scene, except Idle (lobby/main menu).
/// Scene2_SJA covers both the hazard-marking (SJA) and the risk-assessment quiz;
/// both must be completed before the state advances.
/// </summary>
public enum GameState
{
    Idle,           // Main menu / lobby — no active session
    Scene1_PPE,     // PPE kit selection (van)
    Scene2_SJA,     // SJA hazard marking + risk-assessment quiz
    Scene3_PreLift, // Pre-lift preparations on the worksite
    Scene3_1_Lift,  // Aerial lift — actual bulb replacement
    Completed       // Session finished — final score screen
}
