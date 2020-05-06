using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GivePlacesCinemaConsole
{
    public class FillPlacesOverDifferentRows
    {
        private int _neededPlaces;
        private List<Seat> _usedSeats = new List<Seat>();
        private int _rows;
        private int _columns;
        private string[,] _allPlaces;
        private string[,] _bufferAllPLaces;
        private decimal middleRow;
        private decimal middleColumn;
        private string freePlace = "x";
        private string occupiedPlace = "O";
        private Dictionary<Seat, List<Seat>> clusterList = new Dictionary<Seat, List<Seat>>();


        public FillPlacesOverDifferentRows(int neededPlaces, int rows, int columns, string[,] allPlaces)
        {
            _neededPlaces = neededPlaces;
            _rows = rows;
            _columns = columns;
            _allPlaces = CopyArray(allPlaces);
            middleRow = Math.Ceiling((decimal)rows / 2) - 1;
            middleColumn = Math.Ceiling((decimal)columns / 2) - 1;
            _bufferAllPLaces = CopyArray(allPlaces);

            Go();
        }

        private string[,] CopyArray(string[,] array)
        {
            string[,] newArray = new string[_rows, _columns];

            for (int i = 0; i < _rows ; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (array[i,j] == occupiedPlace)
                    {
                        newArray[i, j] = occupiedPlace;
                    }
                    else
                    {
                        newArray[i, j] = freePlace;
                    }
                }
            }

            return newArray;
        }

        private void Go()
        {
            var placesRemaining = _neededPlaces;
            var clusterOfSeats = new List<Seat>();
            // zoek eerste vrij plek volgens de regels. Hoever kan hij nog gaan. 

            var freeSeat = FindOneFreePlace();
            if (freeSeat == null)
            {
                return;
            }

            // clusterOfSeats.Add(freeSeat);

            var listOfFreeSeatsSameRow = ListOfFreeSeatsNextToSeat(freeSeat, placesRemaining);

            foreach (var seat in listOfFreeSeatsSameRow)
            {
                clusterOfSeats.Add(seat);
            }

            placesRemaining -= listOfFreeSeatsSameRow.Count;

            // Dan naar de columns kijken op de rij + 1. clusters zoeken die gescheiden zijn.
            if (placesRemaining > 0)
            {
                var listFreePlacesAboveRow = FindFreePlacesAboveRow(freeSeat.row, listOfFreeSeatsSameRow, placesRemaining);

                foreach (var seat in listFreePlacesAboveRow)
                {
                    clusterOfSeats.Add(seat);
                }

                placesRemaining -= listFreePlacesAboveRow.Count;
            }

            // Dan naar de columns kijken op de rij - 1. clusters zoeken die gescheiden zijn.

            if (placesRemaining > 0)
            {
                var listFreePlacesBellowRow = FindFreePlacesBellowRow(freeSeat.row, listOfFreeSeatsSameRow, placesRemaining);

                foreach (var seat in listFreePlacesBellowRow)
                {
                    clusterOfSeats.Add(seat);
                }

                placesRemaining -= listFreePlacesBellowRow.Count;
            }

            // Dan naar de columns kijken op de rij + 2. clusters zoeken die gescheiden zijn.
            // enz
            // volledige cluster gevonden? opslagen en naar volgende
            if (placesRemaining == 0)
            {
                clusterList.Add(freeSeat, clusterOfSeats);
            }

            FillBufferAllPlaces(clusterOfSeats);

            if (CountEmptyPlaces(_bufferAllPLaces) >= _neededPlaces)
            {
                Go();
            }

            // Slechte clusters ook opslagen zodat er niet onnodig gezocht wordt. 
            // Indien vrije seat in 1 van deze clusters? naar volgende. Altijd deze check doen, ook als je clusters aan het samenstellen bent.
            // Op het einde beste cluster kiezen => zo weinig mogelijk rijen, zo veel mogelijk in het midden (gemiddeld nummer collumns moet zo dicht mogelijk bij middelste liggen. Zelfde bij rij)

        }

        private Seat FindOneFreePlace()
        {
            var freeSeat = new Seat();


            for (int i = (int)middleRow; i < _rows; i++)
            {
                for (int j = (int)middleColumn; j <= (_columns - 1); j++)
                {
                    if (_bufferAllPLaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }

                for (int j = (int)middleColumn; j >= 0; j--)
                {
                    if (_bufferAllPLaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }
            }

            for (int i = (int)middleRow; i >= 0; i--)
            {
                for (int j = (int)middleColumn; j <= (_columns - 1); j++)
                {
                    if (_bufferAllPLaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }

                for (int j = (int)middleColumn; j >= 0; j--)
                {
                    if (_bufferAllPLaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }
            }

            return null;
        }

        private List<Seat> ListOfFreeSeatsNextToSeat(Seat seat, int neededPlaces)
        {
            var listOfSeats = new List<Seat>();

            if (neededPlaces <= 0)
            {
                return listOfSeats;
            }

            for (int i = 0; i < _columns - seat.column; i++)
            {
                if (_allPlaces[seat.row, seat.column + i] == occupiedPlace)
                {
                    break;
                }

                var freeSeat = new Seat();
                freeSeat.column = seat.column + i;
                freeSeat.row = seat.row;
                listOfSeats.Add(freeSeat);

                if (listOfSeats.Count >= neededPlaces)
                {
                    return listOfSeats;
                }
            }

            for (int i = seat.column - 1; i >= 0; i--)
            {
                if (_allPlaces[seat.row, i] == occupiedPlace)
                {
                    break;
                }

                var freeSeat = new Seat();
                freeSeat.column = i;
                freeSeat.row = seat.row;
                listOfSeats.Add(freeSeat);

                if (listOfSeats.Count >= neededPlaces)
                {
                    return listOfSeats;
                }
            }

            return listOfSeats;
        }

        private List<Seat> FindFreePlacesAboveRow(int row, List<Seat> seatsOld, int placesRemaining)
        {
            var seatsNew = new List<Seat>();

            var listOfFreeSeats = new List<Seat>();

            foreach (var seat in seatsOld)
            {
                seatsNew.Add(seat);
            }

            foreach (var seat in seatsNew.Where(s => s.row == row))
            {
                var seatAbove = new Seat { column = seat.column, row = seat.row };
                seatAbove.row--;
                if (seatAbove.row < 0)
                {
                    return new List<Seat>();

                }

                if (_allPlaces[seatAbove.row, seatAbove.column] == freePlace)
                {
                    var freeSeatsAbove = ListOfFreeSeatsNextToSeat(seatAbove, placesRemaining);
                    foreach (var freeSeatAbove in freeSeatsAbove)
                    {
                        if (!listOfFreeSeats.Any(x => x.column == freeSeatAbove.column && x.row == freeSeatAbove.row))
                        {
                            listOfFreeSeats.Add(freeSeatAbove);
                        }
                    }

                    if (listOfFreeSeats.Count < placesRemaining)
                    {
                        int newRow = row - 1;
                        var list = FindFreePlacesAboveRow(newRow, freeSeatsAbove, placesRemaining - listOfFreeSeats.Count);

                        foreach (var seat1 in list)
                        {
                            listOfFreeSeats.Add(seat1);
                        }
                    }
                    else
                    {
                        return listOfFreeSeats;
                    }

                    placesRemaining = placesRemaining - listOfFreeSeats.Count;
                }
            }

            return listOfFreeSeats;
        }

        private List<Seat> FindFreePlacesBellowRow(int row, List<Seat> seatsOld, int placesRemaining)
        {
            var seatsNew = new List<Seat>();

            var listOfFreeSeats = new List<Seat>();

            foreach (var seat in seatsOld)
            {
                seatsNew.Add(seat);
            }

            foreach (var seat in seatsNew.Where(s => s.row == row))
            {
                var seatBellow = new Seat { column = seat.column, row = seat.row };
                seatBellow.row++;

                if (seatBellow.row >= _rows)
                {
                    return new List<Seat>();
                }

                if (_allPlaces[seatBellow.row, seatBellow.column] == freePlace)
                {
                    var freeSeatsBellow = ListOfFreeSeatsNextToSeat(seatBellow, placesRemaining);
                    foreach (var freeSeatBellow in freeSeatsBellow)
                    {
                        if (!listOfFreeSeats.Any(x => x.row == freeSeatBellow.row && x.column == freeSeatBellow.column))
                        {
                            listOfFreeSeats.Add(freeSeatBellow);
                        }
                    }
                    if (listOfFreeSeats.Count < placesRemaining)
                    {
                        int newRow = row + 1;
                        var list = FindFreePlacesBellowRow(newRow, seatsNew, placesRemaining - listOfFreeSeats.Count);
                    }
                    else
                    {
                        return listOfFreeSeats;
                    }
                }
            }
            return listOfFreeSeats;
        }

        private void FillBufferAllPlaces(List<Seat> seats)
        {
            foreach (var seat in seats)
            {
                _bufferAllPLaces[seat.row, seat.column] = occupiedPlace;
            }
        }

        private int CountEmptyPlaces(string[,] allPlaces)
        {
            var count = 0;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (allPlaces[i, j] == freePlace)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public List<Seat> GiveBestCluster()
        {
            return clusterList.Values.FirstOrDefault(x => x.Count == _neededPlaces);
        }
    }
}
