module PrepareFoodTests
open Domain
open States
open Commands
open Events
open CafeAppTestsDSL
open NUnit.Framework
open TestData
open Errors

[<Test>]
let ``Can Prepare Food`` () =
  let order = {order with FoodItems = [salad]}
  let expected = {
      PlacedOrder = order
      ServedDrinks = []
      PreparedFoods = [salad]
      ServedFoods = []}
  Given (PlacedOrder order)
  |> When (PrepareFood (salad,order.TabId))
  |> ThenStateShouldBe (OrderInProgress expected)
  |> WithEvent (FoodPrepared (salad, order.TabId))

[<Test>]
let ``Can not prepare a non-ordered food`` () =
  let order = {order with FoodItems = [pizza]}
  Given (PlacedOrder order)
  |> When (PrepareFood (salad, order.TabId))
  |> ShouldFailWith (CanNotPrepareNonOrderedFood salad)

[<Test>]
let ``Can not prepare a food for served order`` () =
  Given (OrderServed order)
  |> When (PrepareFood (pizza, order.TabId))
  |> ShouldFailWith OrderAlreadyServed

[<Test>]
let ``Can not prepare with closed tab`` () =
  Given (ClosedTab None)
  |> When (PrepareFood (salad, order.TabId))
  |> ShouldFailWith CanNotPrepareWithClosedTab