module Display

open System
open Domain

// 구분선 출력
let printSeparator () =
    printfn "================================================"

// 두꺼운 구분선 출력
let printThickSeparator () =
    printfn "================================================"
    printfn "================================================"

// 게임 타이틀 출력
let printTitle () =
    Console.Clear()
    printfn ""
    printfn "  ⚔️  Legend of Nobody - (TURN-BASED BATTLE GAME)  ⚔️"
    printfn ""
    printThickSeparator ()
    printfn ""

// HP 바 생성 (비주얼 표현)
let createHealthBar (current: int) (max: int) : string =
    let barLength = 20
    let filledLength = 
        if max > 0 then
            int (float barLength * (float current / float max))
        else
            0
    let emptyLength = barLength - filledLength
    
    let filled = String.replicate filledLength "█"
    let empty = String.replicate emptyLength "░"
    
    sprintf "[%s%s]" filled empty

// 캐릭터 상태 출력
let printCharacterStatus (name: string) (character: Character) =
    let healthBar = createHealthBar character.HP character.MaxHP
    let hpColor = 
        if character.HP > character.MaxHP / 2 then
            ConsoleColor.Green
        elif character.HP > character.MaxHP / 4 then
            ConsoleColor.Yellow
        else
            ConsoleColor.Red
    
    printf "%s: " name
    Console.ForegroundColor <- hpColor
    printf "HP %d/%d " character.HP character.MaxHP
    Console.ResetColor()
    printfn "%s | ATK %d" healthBar character.Attack

// 게임 상태 전체 출력
let printGameState (state: GameState) =
    printfn ""
    printCharacterStatus "🧙 Player" state.Player
    printCharacterStatus "👹 Enemy " state.Enemy
    printfn ""
    printSeparator ()

// 행동 선택 메뉴 출력
let printActionMenu () =
    printfn ""
    printfn "Choose your action:"
    printfn "  1. ⚔️  Attack"
    printfn "  2. 🛡️  Defend"
    printf "> "

// 메시지 리스트 출력 (턴 결과)
let printMessages (messages: string list) =
    printfn ""
    printSeparator ()
    messages |> List.iter (fun msg -> 
        printfn "  %s" msg
        // 각 메시지 사이에 약간의 딜레이
        Threading.Thread.Sleep(500)  
    )
    printSeparator ()

// 게임 승리 메시지
let printVictory () =
    Console.Clear()
    printfn ""
    printfn ""
    Console.ForegroundColor <- ConsoleColor.Yellow
    printfn "  ╔════════════════════════════════════╗"
    printfn "  ║                                    ║"
    printfn "  ║          🎉 YOU WIN! 🎉           ║"
    printfn "  ║                                    ║"
    printfn "  ║     You defeated the enemy!        ║"
    printfn "  ║                                    ║"
    printfn "  ╚════════════════════════════════════╝"
    Console.ResetColor()
    printfn ""
    printfn ""

// 게임 오버 메시지
let printGameOver () =
    Console.Clear()
    printfn ""
    printfn ""
    Console.ForegroundColor <- ConsoleColor.Red
    printfn "  ╔════════════════════════════════════╗"
    printfn "  ║                                    ║"
    printfn "  ║        💀 GAME OVER 💀            ║"
    printfn "  ║                                    ║"
    printfn "  ║      You have been defeated...     ║"
    printfn "  ║                                    ║"
    printfn "  ╚════════════════════════════════════╝"
    Console.ResetColor()
    printfn ""
    printfn ""

// 게임 종료 결과 출력
let printGameResult (result: GameResult) =
    match result with
    | PlayerWins -> printVictory ()
    | PlayerLoses -> printGameOver ()
    | Ongoing -> ()  // 진행 중이면 아무것도 출력하지 않음

// 에러 메시지 출력
let printError (message: string) =
    Console.ForegroundColor <- ConsoleColor.Red
    printfn ""
    printfn "Error: %s" message
    Console.ResetColor()
    printfn ""

// 턴 시작 헤더 출력
let printTurnHeader (turnNumber: int) =
    printfn ""
    Console.ForegroundColor <- ConsoleColor.Cyan
    printfn "╔════════════════════════════════════╗"
    printfn "║          TURN %d                    ║" turnNumber
    printfn "╚════════════════════════════════════╝"
    Console.ResetColor()

// 아무 키나 눌러서 계속하기
let waitForKeyPress () =
    printfn ""
    printfn "Press any key to continue..."
    Console.ReadKey(true) |> ignore

// 게임 시작 환영 메시지
let printWelcome () =
    printTitle ()
    printfn "Welcome, brave nobody!"
    printfn ""
    printfn "A fearsome enemy stands before you."
    printfn "Defeat it to claim victory!"
    printfn ""
    printSeparator ()
    waitForKeyPress ()