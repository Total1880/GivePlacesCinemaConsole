using System;
using System.Collections.Generic;
using System.Text;

namespace GivePlacesCinemaConsole
{
    class FindPlace
    {
        int rows = 5;
        int columns = 19;
        private string[,] allPlaces;
        string freePlace = "x";
        string occupiedPlace = "O";
        decimal middleRow;
        decimal middleColumn;
        int totalFreePlaces;

        public FindPlace()
        {
            FillWithEmptySpaces();
            middleRow = Math.Ceiling((decimal)rows / 2) - 1;
            middleColumn = Math.Ceiling((decimal)columns / 2) - 1;
            totalFreePlaces = rows * columns;

            do
            {
                Draw();
                WaitForInput();
            } while (true);
        }

        private void FillWithEmptySpaces()
        {
            allPlaces = new string[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    allPlaces[i, j] = freePlace;
                }
            }
        }

        private void Draw()
        {
            Console.Clear();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write(allPlaces[i, j]);
                }
                Console.WriteLine();
            }
        }

        private void WaitForInput()
        {
            int input;
            if (totalFreePlaces == 0)
            {
                Console.WriteLine("Soldout");
                Console.ReadLine();
                return;
            }

            Console.Write("How many places do you need: ");

            if (!int.TryParse(Console.ReadLine(), out input))
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
            //Primary: Search free places on the same row
            var allPlacesFound = FreePlacesOnSameRow(neededPlaces);
            if (!allPlacesFound)
            {
                //Algoritme om over verschillende rijen te zoeken
                var fillPlacesOverDifferentRows = new FillPlacesOverDifferentRows(neededPlaces, rows, columns, allPlaces);
                var foundCluster = fillPlacesOverDifferentRows.GiveBestCluster();
                if (foundCluster != null)
                {
                    fillFreePlacesWithList(foundCluster);
                }
                else
                {
                    Console.WriteLine("no cluster found");
                    Console.ReadLine();
                }
            }
        }

        private bool FreePlacesOnSameRow(int neededPlaces)
        {
            bool placeFound = false;

            for (int i = (int)middleRow; i >= 0; i--)
            {
                placeFound = SearchFreePlacesInRows(neededPlaces, i);

                if (placeFound)
                    break;
            }

            if (!placeFound)
            {
                for (int i = (int)middleRow; i < rows; i++)
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
            bool placeFound = false;
            List<decimal> devidedNeededPlaces = new List<decimal>();

            //nakijken of ze in het midden bij elkaar kunnen zitten
            if (allPlaces[row, (int)middleColumn] == freePlace && neededPlaces > (columns - middleColumn) && neededPlaces <= columns)
            {
                //Eerst ceiling zetten!
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

                for (int i = (int)middleColumn; i <= (columns - places); i++)
                {
                    if (allPlaces[row, i] == freePlace)
                    {
                        Console.WriteLine("Free place found!!!");
                        fillFreePlaces(row, i, places);
                        placeFound = true;
                        break;
                    }
                }

                if (!placeFound)
                {
                    for (int i = (int)middleColumn; i >= (places - 1); i--)
                    {
                        if (allPlaces[row, i] == freePlace)
                        {
                            Console.WriteLine("Free place found!!!");
                            fillFreePlaces(row, i - (places - 1), places);
                            placeFound = true;
                            break;
                        }
                    }
                }
            }

            return placeFound;
        }

        private void fillFreePlaces(int row, int column, int places)
        {
            for (int i = column; i < column + places; i++)
            {
                allPlaces[row, i] = occupiedPlace;
            }
        }

        private void fillFreePlacesWithList(List<Seat> seats)
        {
            foreach (var seat in seats)
            {
                allPlaces[seat.row, seat.column] = occupiedPlace;
            }
        }
    }
}
