﻿module AngouriMath.FSharp.Matrices

open AngouriMath
open AngouriMath.FSharp.Core
open AngouriMath.FSharp.Functions
open System.Linq

/// Creates a matrix from a list of lists
/// of objects (which are parsed into Entities)
let matrix x = 
 let rec columnCount (x : 'T list list) =
     match x with
     | [] -> Any
     | hd::tl ->
         match columnCount tl with
         | Any -> Fixed(hd.Length)
         | Invalid -> Invalid
         | Fixed len -> if len = hd.Length then Fixed(len) else Invalid
 
 let parseListOfLists li =
     [ for row in li do yield [ for el in row do yield (parsed el) ] ]
 
 match columnCount x with
 | Any | Invalid -> raise ParseException
 | Fixed _ -> MathS.Matrix(array2D (parseListOfLists x))
 
 
/// Creates a column vector from a 1-dimensional list
let vector li =
 MathS.Vector([ for el in li do yield parsed el ].ToArray())

/// If the provided entity is a matrix,
/// it is returned downcasted. Otherwise,
/// a 1x1 matrix containing the entity is returned.
let asMatrix a = 
 match parsed a with
 | :? Entity.Matrix as m -> m
 | other -> vector [other]

/// Creates a square 2x2 matrix, with four elements
/// in the following order: left-top, right-top,
/// left-bottom, right-bottom
let matrix2x2 leftTop rightTop leftBottom rightBottom = 
    matrix [[leftTop; rightTop]; [leftBottom; rightBottom]]
 
/// Creates a square 3x3 matrix, going first from left
/// to right, then from top to bottom (the same way
/// people in Europe read)
let matrix3x3 leftTop middleTop rightTop leftMiddle middleMiddle rightMiddle leftBottom middleBottom rightBottom =
    matrix [[leftTop; middleTop; rightTop]; [leftMiddle; middleMiddle; rightMiddle]; [leftBottom; middleBottom; rightBottom]]
 
/// Creates a vector of two elements
let vector2 a b = vector [a; b]
 
/// Creates a vector of three elements
let vector3 a b c = vector [a; b; c]
 
/// Creates a new matrix with each
/// cell's coordinates mapped to
/// an element (first comes the number of
/// the row, then the number of column,
/// starting from 0)
let matrixWith rows cols map = MathS.Matrix(rows, cols, new System.Func<int, int, Entity>(map))
 
/// Creates a new vector with
/// each element mapped from its
/// position (indexed from 0)
/// to the value via the map function
let vectorWith count map = matrixWith count 1 (fun r _ -> map r)

let (|*) a b = (parsed a * parsed b).InnerSimplified |> asMatrix

let (|**) a b = (parsed a ** parsed b).InnerSimplified |> asMatrix

let (|+) a b = (parsed a + parsed b).InnerSimplified |> asMatrix

let (|-) a b = (parsed a - parsed b).InnerSimplified |> asMatrix

let (|/) a b = (parsed a / parsed b).InnerSimplified |> asMatrix

/// Finds the tensor product of two given matrices
let ( *** ) a b =
 Entity.Matrix.TensorProduct(a, b).InnerSimplified |> asMatrix

/// Finds the tensor power of a given matrix. The
/// power must be positive
let ( **** ) (a: Entity.Matrix) b =
 a.TensorPower(b).InnerSimplified |> asMatrix

/// Returns a matrix modified according to the modifier
let modifiedMatrix (x: AngouriMath.Entity.Matrix) m =
    x.With(new System.Func<int, int, AngouriMath.Entity, AngouriMath.Entity>(m))

/// Gets the transposed form of a matrix or vector
let transposed (m: AngouriMath.Entity.Matrix) = m.T
