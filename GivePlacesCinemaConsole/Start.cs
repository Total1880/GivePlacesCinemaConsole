using System;
using System.Collections.Generic;
using System.Text;

namespace GivePlacesCinemaConsole
{
    public class Start
    {
        int rows = 5;
        int columns = 19;
        private string[,] allPlaces;
        string freePlace = "x";
        string occupiedPlace = "O";
        decimal middleRow;
        decimal middleColumn;
        int totalFreePlaces;

        public Start()
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
            List<int> devidedNeededPlaces = new List<int>() { neededPlaces };
            List<decimal> devidedNeededPlacesBuffer = new List<decimal>();
            bool soldout = false;
            bool firstTime = true;
            
            //todo als de plaatsen worden gesplitst over, over de rijden heen bij elkaar zetten
            //nakijken of er wel genoeg plaats is
            if (totalFreePlaces < neededPlaces)
            {
                Console.WriteLine($"Only {totalFreePlaces} places left. Press enter to try again.");
                Console.ReadLine();
                return;
            }

            do
            {
                foreach (var places in devidedNeededPlaces)
                {
                    if (places > columns || !SearchFreePlacesSingleInput(places))
                    {
                        if (firstTime)
                        {
                            Console.WriteLine("Places will be split up");
                            Console.ReadLine();
                            firstTime = false;
                        }

                        devidedNeededPlacesBuffer.Add(Math.Floor((decimal)places / 2));
                        devidedNeededPlacesBuffer.Add(Math.Ceiling((decimal)places / 2));
                    }
                }

                devidedNeededPlaces.Clear();

                foreach (var item in devidedNeededPlacesBuffer)
                {
                    devidedNeededPlaces.Add((int)item);
                }

                devidedNeededPlacesBuffer.Clear();
            } while (!soldout && devidedNeededPlaces.Count != 0);

            totalFreePlaces -= neededPlaces;
        }

        private bool SearchFreePlacesSingleInput(int neededPlaces)
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
            if (allPlaces[row,(int)middleColumn] == freePlace && neededPlaces > (columns - middleColumn))
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

        private void fillFreePlaces(int row, int column, int places)
        {
            for (int i = column; i < column + places; i++)
            {
                allPlaces[row, i] = occupiedPlace;
            }
        }
    }
}
