module GameLogic

open Domain

// 공격 데미지 계산
// 크리티컬 히트
let calculateAttackDamage (attacker: Character) : int =
    let baseDamage = attacker.Attack
    let random = System.Random()
    
    // 20% 확률로 크리티컬 
    if random.Next(100) < 20 then  
        baseDamage * 2  
    else
        baseDamage

// 방어 시 데미지 감소율 계산 (50% 감소)
let calculateDefenseReduction () : float =
    0.5

// 데미지를 받은 후 새로운 캐릭터 상태 반환
let applyDamage (character: Character) (damage: int) : Character =
    let newHP = max 0 (character.HP - damage)
    { character with HP = newHP }

// 플레이어의 공격 처리
let processPlayerAttack (state: GameState) : GameState * int =
    let damage = calculateAttackDamage state.Player
    let newEnemy = applyDamage state.Enemy damage
    let newState = { state with Enemy = newEnemy; IsPlayerDefending = false }
    (newState, damage)

// 플레이어의 방어 처리
let processPlayerDefend (state: GameState) : GameState =
    { state with IsPlayerDefending = true }

// 플레이어 행동 처리
let processPlayerAction (state: GameState) (action: Action) : GameState * string list * int option =
    match action with
    | Attack ->
        let (newState, damage) = processPlayerAttack state
        let messages = [
            "You attacked the enemy!"
            sprintf "Enemy took %d damage!" damage
        ]
        (newState, messages, Some damage)
    | Defend ->
        let newState = processPlayerDefend state
        let messages = ["You brace for the enemy's attack!"]
        (newState, messages, None)

// 적의 공격 처리
let processEnemyAttack (state: GameState) : GameState * int * string list =
    let baseDamage = calculateAttackDamage state.Enemy
    
    // 플레이어가 방어 중이면 데미지 감소
    let actualDamage = 
        if state.IsPlayerDefending then
            let reduction = calculateDefenseReduction ()
            int (float baseDamage * (1.0 - reduction))
        else
            baseDamage
    
    let newPlayer = applyDamage state.Player actualDamage
    let newState = { state with Player = newPlayer; IsPlayerDefending = false }
    
    let messages = 
        if state.IsPlayerDefending then
            [
                "Enemy attacks!"
                sprintf "Your defense reduced the damage! You took %d damage!" actualDamage
            ]
        else
            [
                "Enemy attacks!"
                sprintf "You took %d damage!" actualDamage
            ]
    
    (newState, actualDamage, messages)




// 한 턴 전체 처리
let processTurn (state: GameState) (playerAction: Action) : TurnResult =
    // 1. 플레이어 행동 처리
    let (stateAfterPlayer, playerMessages, playerDamage) = 
        processPlayerAction state playerAction
    
    // 2. 적이 살아있는지 확인
    if not (isAlive stateAfterPlayer.Enemy) then
        // 적이 죽었으면 적은 행동하지 않음
        {
            NewState = stateAfterPlayer
            PlayerDamageDealt = playerDamage
            PlayerDamageTaken = None
            Message = playerMessages
        }
    else
        // 3. 적 행동 처리
        let (finalState, enemyDamage, enemyMessages) = 
            processEnemyAttack stateAfterPlayer
        
        {
            NewState = finalState
            PlayerDamageDealt = playerDamage
            PlayerDamageTaken = Some enemyDamage
            Message = playerMessages @ enemyMessages
        }

// 게임 진행 가능 여부 확인
let canContinue (state: GameState) : bool =
    match checkGameResult state with
    | Ongoing -> true
    | _ -> false

// 게임 종료 메시지 가져오기
let getGameEndMessage (state: GameState) : string option =
    match checkGameResult state with
    | PlayerWins -> Some "🎉 You Win! 🎉"
    | PlayerLoses -> Some "💀 Game Over 💀"
    | Ongoing -> None