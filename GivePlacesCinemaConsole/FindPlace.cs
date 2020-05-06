using System;
using System.Collections.Generic;
using System.Linq;

namespace GivePlacesCinemaConsole
{
    class FindPlace
    {
        private string[,] _allPlaces;
        private readonly decimal _middleRow;
        private readonly decimal _middleColumn;

        public FindPlace()
        {
            FillWithEmptySpaces();
            _middleRow = Math.Ceiling((decimal)Attributes.Rows / 2) - 1;
            _middleColumn = Math.Ceiling((decimal)Attributes.Columns / 2) - 1;

            do
            {
                Draw();
                WaitForInput();
            } while (true);
        }

        private void FillWithEmptySpaces()
        {
            _allPlaces = new string[Attributes.Rows, Attributes.Columns];

            for (var i = 0; i < Attributes.Rows; i++)
            {
                for (var j = 0; j < Attributes.Columns; j++)
                {
                    _allPlaces[i, j] = Attributes.FreePlace;
                }
            }
        }

        private void Draw()
        {
            Console.Clear();
            for (var i = 0; i < Attributes.Rows; i++)
            {
                for (var j = 0; j < Attributes.Columns; j++)
                {
                    Console.Write(_allPlaces[i, j]);
                }
                Console.WriteLine();
            }
        }

        private void WaitForInput()
        {
            if (CheckIfSoldOut())
            {
                Console.WriteLine("Soldout");
                Console.ReadLine();
                return;
            }

            Console.Write("How many places do you need: ");

            if (!int.TryParse(Console.ReadLine(), out var input))
            {
                Console.WriteLine("---Invalid Input---");
                Console.ReadLine();
            }
            else
            {
                SearchFreePlaces(input);
            }
        }

        private void SearchFreePlaces(int neededPlaces)
        {
            //Zoek eerst vrije plaatsen op dezelfde rij
            var allPlacesFound = FreePlacesOnSameRow(neededPlaces);

            if (!allPlacesFound)
            {
                //Algoritme om over verschillende rijen te zoeken
                var fillPlacesOverDifferentRows = new FillPlacesOverDifferentRows(neededPlaces, Attributes.Rows, Attributes.Columns, _allPlaces);

                var foundCluster = fillPlacesOverDifferentRows.GiveBestCluster();

                if (foundCluster != null)
                {
                    FillFreePlacesWithList(foundCluster);
                }
                else
                {
                    Console.WriteLine("No cluster found. Press enter.");
                    Console.ReadLine();
                }
            }
        }

        private bool FreePlacesOnSameRow(int neededPlaces)
        {
            var placeFound = false;

            for (var i = (int)_middleRow; i >= 0; i--)
            {
                placeFound = SearchFreePlacesInRows(neededPlaces, i);

                if (placeFound)
                    break;
            }

            if (!placeFound)
            {
                for (var i = (int)_middleRow; i < Attributes.Rows; i++)
                {
                    placeFound = SearchFreePlacesInRows(neededPlaces, i);

                    if (placeFound)
                        break;
                }
            }

            return placeFound;
        }

        private bool SearchFreePlacesInRows(int neededPlaces, int row)
        {
            var placeFound = false;
            var devidedNeededPlaces = new List<decimal>();

            //nakijken of ze in het midden bij elkaar kunnen zitten
            if (_allPlaces[row, (int)_middleColumn] == Attributes.FreePlace && neededPlaces > (Attributes.Columns - _middleColumn) && neededPlaces <= Attributes.Columns)
            {
                devidedNeededPlaces.Add(Math.Ceiling((decimal)neededPlaces / 2));
                devidedNeededPlaces.Add(Math.Floor((decimal)neededPlaces / 2));
            }
            else
            {
                devidedNeededPlaces.Add(neededPlaces);
            }

            foreach (int places in devidedNeededPlaces)
            {
                placeFound = false;

                for (var i = (int)_middleColumn; i <= (Attributes.Columns - places); i++)
                {
                    if (_allPlaces[row, i] == Attributes.FreePlace)
                    {
                        Console.WriteLine("Free place found!!!");
                        FillFreePlaces(row, i, places);
                        placeFound = true;
                        break;
                    }
                }

                if (!placeFound)
                {
                    for (var i = (int)_middleColumn; i >= (places - 1); i--)
                    {
                        if (_allPlaces[row, i] == Attributes.FreePlace)
                        {
                            Console.WriteLine("Free place found!!!");
                            FillFreePlaces(row, i - (places - 1), places);
                            placeFound = true;
                            break;
                        }
                    }
                }
            }

            return placeFound;
        }

        private void FillFreePlaces(int row, int column, int places)
        {
            for (var i = column; i < column + places; i++)
            {
                _allPlaces[row, i] = Attributes.OccupiedPlace;
            }
        }

        private void FillFreePlacesWithList(IEnumerable<Seat> seats)
        {
            foreach (var seat in seats)
            {
                _allPlaces[seat.row, seat.column] = Attributes.OccupiedPlace;
            }
        }

        private bool CheckIfSoldOut()
        {
            for (var i = 0; i < Attributes.Rows; i++)
            {
                for (var j = 0; j < Attributes.Columns; j++)
                {
                    if (_allPlaces[i,j] == Attributes.FreePlace)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
