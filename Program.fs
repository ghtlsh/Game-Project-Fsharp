module Program

open System
open Domain
open GameLogic
open Display
open Input

// 한 턴 실행
let executeTurn (state: GameState) (turnNumber: int) : GameState =
    // 턴 헤더 출력
    printTurnHeader turnNumber
    
    // 현재 게임 상태 출력
    printGameState state
    
    // 플레이어 행동 선택
    let playerAction = getPlayerAction ()
    
    // 턴 처리
    let turnResult = processTurn state playerAction
    
    // 턴 결과 메시지 출력
    printMessages turnResult.Message
    
    // 새로운 상태 반환
    turnResult.NewState

// 게임 메인 루프
let rec gameLoop (state: GameState) (turnNumber: int) : GameResult =
    // 턴 실행
    let newState = executeTurn state turnNumber
    
    // 게임 종료 조건 체크
    let result = checkGameResult newState
    
    match result with
    | Ongoing -> 
        // 게임 계속 진행
        Threading.Thread.Sleep(1000)  // 다음 턴 전 1초 대기
        gameLoop newState (turnNumber + 1)
    | _ -> 
        // 게임 종료
        result

// 단일 게임 실행
let playGame () =
    // 환영 메시지
    printWelcome ()
    
    // 게임 초기화
    let initialState = createInitialGameState ()
    
    // 게임 시작
    printfn ""
    printfn "The battle begins!"
    waitForKeyPress ()
    
    // 게임 루프 실행
    let result = gameLoop initialState 1
    
    // 최종 결과 출력
    printGameResult result
    
    // 결과 반환
    result

// 게임 재시작 루프
let rec mainLoop () =
    // 게임 실행
    let _ = playGame ()
    
    // 재시작 여부 확인
    if askPlayAgain () then
        Console.Clear()
        mainLoop ()
    else
        printfn ""
        printfn "Thanks for playing! Goodbye! 👋"
        printfn ""

// 프로그램 진입점
[<EntryPoint>]
let main argv =
    try
        // 콘솔 인코딩 설정 (이모지 출력을 위해)
        Console.OutputEncoding <- Text.Encoding.UTF8
        
        // 메인 루프 시작
        mainLoop ()
        
        // 정상 종료
        0
    with
    | ex ->
        // 예외 처리
        Console.ForegroundColor <- ConsoleColor.Red
        printfn ""
        printfn "An error occurred: %s" ex.Message
        printfn ""
        Console.ResetColor()
        
        // 에러 코드 반환
        1