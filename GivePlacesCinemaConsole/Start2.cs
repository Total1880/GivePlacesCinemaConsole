using System;
using System.Collections.Generic;
using System.Text;

namespace GivePlacesCinemaConsole
{
    class Start2
    {
        int rows = 5;
        int columns = 19;
        private string[,] allPlaces;
        string freePlace = "x";
        string occupiedPlace = "O";
        decimal middleRow;
        decimal middleColumn;
        int totalFreePlaces;

        public Start2()
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
            bool soldout = false;
            bool firstTime = true;

            if (totalFreePlaces < neededPlaces)
            {
                Console.WriteLine($"Only {totalFreePlaces} places left. Press enter to try again.");
                Console.ReadLine();
                return;
            }

            while (!SearchFreePlacesSingleInput(neededPlaces))
            {

            }

            totalFreePlaces -= neededPlaces;
        }

        private bool SearchFreePlacesSingleInput(int neededPlaces)
        {
            bool placeFound = false;

            //van middelste rij naar boven
            for (int i = (int)middleRow; i >= 0; i--)
            {
                placeFound = SearchFreePlacesInRows(neededPlaces, i);

                if (placeFound)
                    break;
            }

            //van middelste rij naar beneden
            if (!placeFound)
            {
                for (int i = (int)middleRow; i < rows; i++)
                {
                    placeFound = SearchFreePlacesInRows(neededPlaces, i);

                    if (placeFound)
                        break;
                }
            }

            //Indien niet naast elkaar, over verschillende rijen bij elkaar
            //mms met bool in search freeplacesinrows? 
            SearchFreePlacesOverDifferentRows(neededPlaces);

            return placeFound;
        }

        private bool SearchFreePlacesInRows(int neededPlaces, int row)
        {
            bool placeFound = false;
            List<decimal> devidedNeededPlaces = new List<decimal>();

            //nakijken of ze in het midden bij elkaar kunnen zitten
            if (allPlaces[row, (int)middleColumn] == freePlace && neededPlaces > (columns - middleColumn))
            {
                devidedNeededPlaces.Add(Math.Floor((decimal)neededPlaces / 2));
                devidedNeededPlaces.Add(Math.Ceiling((decimal)neededPlaces / 2));
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

        private void SearchFreePlacesOverDifferentRows(int places)
        {
            for (int i = 0; i < 1; i++)
            {

            }
        }

        private void FindOneFreePlace(int places)
        {
            for (int i = (int)middleRow; i >= 0; i--)
            {
                for (int j = (int)middleColumn; j <= (columns - 1); j++)
                {
                    if (allPlaces[i, j] == freePlace)
                    {
                        var occupiedPlaces = maxPlacesOnRow(i, j, places);
                        
                        break;
                    }
                }

                for (int j = (int)middleColumn; j >= 0; i--)
                {
                    if (allPlaces[i, j] == freePlace)
                    {
                        break;
                    }
                }
            }

            for (int i = (int)middleRow; i < rows; i++)
            {
                for (int j = (int)middleColumn; j <= (columns - 1); j++)
                {
                    if (allPlaces[i, j] == freePlace)
                    {
                        break;
                    }
                }

                for (int j = (int)middleColumn; j >= 0; i--)
                {
                    if (allPlaces[i, j] == freePlace)
                    {
                        break;
                    }
                }
            }
        }

        private int maxPlacesOnRow(int row, int column, int places)
        {
            var count = 1;
            var connectingColumns = new List<int>();

            for (int i = column; i < columns; i++)
            {
                if (allPlaces[row,i] == freePlace && count < places)
                {
                    connectingColumns.Add(i);
                    count++;
                }
                else
                {
                    var temp = FindPlacesFromManualColumn(row + 1, connectingColumns, places - count);
                }
            }
            return count;
        }

        private int FindPlacesFromManualColumn(int row, List<int> connectingColumns, int places)
        {
            var count = 0;

            if (row <= rows || row >= 0)
            {
                for (int i = 0; i < columns; i++)
                {
                    if (i == 0)
                    {

                    }
                }
            }
            return count;
        }

        private void fillFreePlaces(int row, int column, int places)
        {
            for (int i = column; i < column + places; i++)
            {
                allPlaces[row, i] = occupiedPlace;
            }
        }
    }
}
