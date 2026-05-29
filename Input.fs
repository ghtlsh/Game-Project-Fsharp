module Input

open System
open Domain
open Display

// 문자열을 정수로 변환 시도
let tryParseInt (input: string) : int option =
    match Int32.TryParse(input) with
    | (true, value) -> Some value
    | (false, _) -> None

// 입력이 유효한 액션 번호인지 확인
let isValidActionNumber (number: int) : bool =
    number = 1 || number = 2

// 숫자를 Action 타입으로 변환
let numberToAction (number: int) : Action option =
    match number with
    | 1 -> Some Attack
    | 2 -> Some Defend
    | _ -> None

// 사용자로부터 한 줄 입력 받기
let readLine () : string =
    Console.ReadLine().Trim()

// 사용자 입력을 Action으로 파싱
let parseAction (input: string) : Action option =
    input
    |> tryParseInt
    |> Option.bind numberToAction

// 유효한 입력을 받을 때까지 반복
let rec getPlayerAction () : Action =
    printActionMenu ()
    let input = readLine ()
    
    match parseAction input with
    | Some action -> 
        action
    | None -> 
        printError "Invalid input! Please enter 1 or 2."
        getPlayerAction ()  // 재귀 호출로 다시 입력 받기

// Yes/No 질문 (나중에 재시작 기능 등에 활용)
let rec askYesNo (question: string) : bool =
    printfn "%s (y/n)" question
    printf "> "
    let input = readLine().ToLower()
    
    match input with
    | "y" | "yes" -> true
    | "n" | "no" -> false
    | _ -> 
        printError "Please enter 'y' or 'n'."
        askYesNo question

// 게임 재시작 여부 확인
let askPlayAgain () : bool =
    printfn ""
    askYesNo "Do you want to play again?"

// 게임 시작 전 준비 확인
let waitForGameStart () =
    printfn ""
    printfn "Press any key to start the battle..."
    Console.ReadKey(true) |> ignore
    Console.Clear()