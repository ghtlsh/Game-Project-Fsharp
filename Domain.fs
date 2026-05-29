module Domain

// 캐릭터 타입 (플레이어와 적 공통)
type Character = {
    HP: int
    MaxHP: int
    Attack: int
}

// 플레이어가 선택할 수 있는 행동
type Action = 
    | Attack 
    | Defend

// 게임의 현재 상태
type GameState = {
    Player: Character
    Enemy: Character
    IsPlayerDefending: bool  // 플레이어가 이번 턴에 방어 중인지
}

// 게임 종료 결과
type GameResult =
    | PlayerWins
    | PlayerLoses
    | Ongoing

// 턴 결과 (한 턴이 끝난 후의 정보)
type TurnResult = {
    NewState: GameState
    PlayerDamageDealt: int option    // 플레이어가 입힌 데미지
    PlayerDamageTaken: int option    // 플레이어가 받은 데미지
    Message: string list             // 턴 동안 발생한 메시지들
}

// 초기 캐릭터 생성 함수
let createPlayer () = {
    HP = 30
    MaxHP = 30
    Attack = 5
}

let createEnemy () = {
    HP = 20
    MaxHP = 20
    Attack = 4
}

// 초기 게임 상태 생성
let createInitialGameState () = {
    Player = createPlayer ()
    Enemy = createEnemy ()
    IsPlayerDefending = false
}

// 게임 종료 조건 체크
let checkGameResult (state: GameState) : GameResult =
    if state.Player.HP <= 0 then
        PlayerLoses
    elif state.Enemy.HP <= 0 then
        PlayerWins
    else
        Ongoing

// 캐릭터가 살아있는지 확인
let isAlive (character: Character) =
    character.HP > 0