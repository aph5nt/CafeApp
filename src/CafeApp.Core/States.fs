module States
open Domain
open System
open Events

type State =
  | ClosedTab of Guid option
  | OpenedTab of Tab
  | PlacedOrder of Order
  | OrderInProgress of InProgressOrder
  | OrderServed of Order

let getState (ipo : InProgressOrder) =
  if isOrderServed ipo then
    OrderServed ipo.PlacedOrder
  else
    OrderInProgress ipo

let apply state event =
  match state,event with
  | ClosedTab _, TabOpened tab -> OpenedTab tab
  | OpenedTab _, OrderPlaced order -> PlacedOrder order
  | PlacedOrder order, DrinksServed (item,_) ->
    {
      PlacedOrder = order
      ServedDrinks = [item]
      ServedFoods = []
      PreparedFoods = []
    } |> getState
  | OrderInProgress ipo, DrinksServed (item,_) ->
    {ipo with ServedDrinks = item :: ipo.ServedDrinks}
    |> getState
  | PlacedOrder order, FoodPrepared (item,_) ->
    {
      PlacedOrder = order
      PreparedFoods = [item]
      ServedDrinks = []
      ServedFoods = []
    } |> OrderInProgress
  | _ -> state