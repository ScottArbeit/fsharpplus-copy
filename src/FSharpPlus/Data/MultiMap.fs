﻿namespace FSharpPlus.Data

open System.ComponentModel
open System.Collections.Generic
open FSharpPlus

/// A Map where keys are not unique.
type MultiMap<'Key, 'Value when 'Key : comparison> = private MMap of Map<'Key, 'Value list> with

    interface IEnumerable<KeyValuePair<'Key, 'Value>> with
        member x.GetEnumerator () =
            let (MMap x) = x
            (x |> Map.toSeq |> Seq.collect (fun (k, v) -> v |> List.map (fun v -> KeyValuePair (k, v)))).GetEnumerator ()

    interface System.Collections.IEnumerable with
        member x.GetEnumerator () =
            let (MMap x) = x
            (x |> Map.toSeq |> Seq.collect (fun (k, v) -> v |> List.map (fun v -> KeyValuePair (k, v)))).GetEnumerator () :> System.Collections.IEnumerator

    member x.Item
        with get (key: 'Key) : 'Value list =
            let (MMap x) = x
            match Map.tryFind key x with
            | Some values -> values
            | None        -> []

    static member get_Zero () = MMap (Map.ofList [])
    static member (+) (MMap x, MMap y) = MMap (Map.unionWith (@) x y)


module MultiMap =

    /// Converts a list of tuples to a MultiMap.
    let ofList (x: list<'Key * 'Value>) = x |> Seq.groupBy fst |> Seq.map (fun (k, v) -> k, v |> Seq.map snd |> Seq.toList) |> Map.ofSeq |> MMap

    /// Converts a MultiMap to a list of tuples.
    let toList (MMap x) = x :> seq<_> |> Seq.collect (fun x -> x.Value |> List.map (fun v -> (x.Key, v)))


type MultiMap<'Key, 'Value when 'Key : comparison> with

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member ToList x = MultiMap.toList x

    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member OfList x = MultiMap.ofList x