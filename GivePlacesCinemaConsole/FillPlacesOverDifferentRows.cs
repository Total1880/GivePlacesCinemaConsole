using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace GivePlacesCinemaConsole
{
    public class FillPlacesOverDifferentRows
    {
        private readonly int _neededPlaces;
        private readonly int _rows;
        private readonly int _columns;
        private readonly string[,] _allPlaces;
        private readonly string[,] _bufferAllPLaces;
        private readonly decimal _middleRow;
        private readonly decimal _middleColumn;
        private readonly Dictionary<Seat, List<Seat>> _clusterList = new Dictionary<Seat, List<Seat>>();


        public FillPlacesOverDifferentRows(int neededPlaces, int rows, int columns, string[,] allPlaces)
        {
            _neededPlaces = neededPlaces;
            _rows = rows;
            _columns = columns;
            _allPlaces = CopyArray(allPlaces);
            _middleRow = Math.Ceiling((decimal)rows / 2) - 1;
            _middleColumn = Math.Ceiling((decimal)columns / 2) - 1;
            _bufferAllPLaces = CopyArray(allPlaces);

            Go();
        }

        private string[,] CopyArray(string[,] array)
        {
            var newArray = new string[_rows, _columns];

            for (var i = 0; i < _rows ; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    if (array[i,j] == Attributes.OccupiedPlace)
                    {
                        newArray[i, j] = Attributes.OccupiedPlace;
                    }
                    else
                    {
                        newArray[i, j] = Attributes.FreePlace;
                    }
                }
            }

            return newArray;
        }

        private void Go()
        {
            var placesRemaining = _neededPlaces;

            // zoek eerste vrij plek volgens de regels. 
            var freeSeat = FindOneFreePlace();
            if (freeSeat == null)
            {
                return;
            }

            // Hoeveel plaatsen op de eerste rij zijn er.
            var listOfFreeSeatsSameRow = ListOfFreeSeatsNextToSeat(freeSeat, placesRemaining);

            var clusterOfSeats = listOfFreeSeatsSameRow.ToList();

            placesRemaining -= listOfFreeSeatsSameRow.Count;

            //Cluster zoeken met de plaatsen erboven
            if (placesRemaining > 0)
            {
                var listFreePlacesAboveRow = FindFreePlacesAboveRow(freeSeat.row, listOfFreeSeatsSameRow, placesRemaining);

                clusterOfSeats.AddRange(listFreePlacesAboveRow);

                placesRemaining -= listFreePlacesAboveRow.Count;
            }

            //Cluster zoeken met de plaatsen eronder
            if (placesRemaining > 0)
            {
                var listFreePlacesBellowRow = FindFreePlacesBellowRow(freeSeat.row, listOfFreeSeatsSameRow, placesRemaining);

                clusterOfSeats.AddRange(listFreePlacesBellowRow);

                placesRemaining -= listFreePlacesBellowRow.Count;
            }

            // volledige cluster gevonden? opslagen en naar volgende.
            if (placesRemaining == 0)
            {
                _clusterList.Add(freeSeat, clusterOfSeats);
            }

            FillBufferAllPlaces(clusterOfSeats);

            //Blijven zoeken tot hij alle plaatsen heeft afgezocht
            if (CountEmptyPlaces(_bufferAllPLaces) >= _neededPlaces)
            {
                Go();
            }
        }

        private Seat FindOneFreePlace()
        {
            var freeSeat = new Seat();

            for (var i = (int)_middleRow; i < _rows; i++)
            {
                for (var j = (int)_middleColumn; j <= (_columns - 1); j++)
                {
                    if (_bufferAllPLaces[i, j] == Attributes.FreePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }

                for (var j = (int)_middleColumn; j >= 0; j--)
                {
                    if (_bufferAllPLaces[i, j] == Attributes.FreePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }
            }

            for (var i = (int)_middleRow; i >= 0; i--)
            {
                for (var j = (int)_middleColumn; j <= (_columns - 1); j++)
                {
                    if (_bufferAllPLaces[i, j] == Attributes.FreePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }

                for (var j = (int)_middleColumn; j >= 0; j--)
                {
                    if (_bufferAllPLaces[i, j] == Attributes.FreePlace)
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

            for (var i = 0; i < _columns - seat.column; i++)
            {
                if (_allPlaces[seat.row, seat.column + i] == Attributes.OccupiedPlace)
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

            for (var i = seat.column - 1; i >= 0; i--)
            {
                if (_allPlaces[seat.row, i] == Attributes.OccupiedPlace)
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

        private List<Seat> FindFreePlacesAboveRow(int row, List<Seat> seats, int placesRemaining)
        {
            var listOfFreeSeats = new List<Seat>();

            foreach (var seat in seats.Where(s => s.row == row))
            {
                var seatAbove = new Seat { column = seat.column, row = seat.row };
                seatAbove.row--;
                if (seatAbove.row < 0)
                {
                    return new List<Seat>();

                }

                if (_allPlaces[seatAbove.row, seatAbove.column] == Attributes.FreePlace)
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
                        var newRow = row - 1;
                        var list = FindFreePlacesAboveRow(newRow, freeSeatsAbove, placesRemaining - listOfFreeSeats.Count);

                        listOfFreeSeats.AddRange(list);
                    }
                    else
                    {
                        return listOfFreeSeats;
                    }

                    placesRemaining -= listOfFreeSeats.Count;
                }
            }

            return listOfFreeSeats;
        }

        private List<Seat> FindFreePlacesBellowRow(int row, List<Seat> seats, int placesRemaining)
        {
            var listOfFreeSeats = new List<Seat>();

            foreach (var seat in seats.Where(s => s.row == row))
            {
                var seatBellow = new Seat { column = seat.column, row = seat.row };
                seatBellow.row++;

                if (seatBellow.row >= _rows)
                {
                    return new List<Seat>();
                }

                if (_allPlaces[seatBellow.row, seatBellow.column] == Attributes.FreePlace)
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
                        var newRow = row + 1;
                        var list = FindFreePlacesBellowRow(newRow, freeSeatsBellow, placesRemaining - listOfFreeSeats.Count);

                        listOfFreeSeats.AddRange(list);
                    }
                    else
                    {
                        return listOfFreeSeats;
                    }

                    placesRemaining -= listOfFreeSeats.Count;
                }
            }
            return listOfFreeSeats;
        }

        private void FillBufferAllPlaces(List<Seat> seats)
        {
            foreach (var seat in seats)
            {
                _bufferAllPLaces[seat.row, seat.column] = Attributes.OccupiedPlace;
            }
        }

        private int CountEmptyPlaces(string[,] allPlaces)
        {
            var count = 0;
            for (var i = 0; i < _rows; i++)
            {
                for (var j = 0; j < _columns; j++)
                {
                    if (allPlaces[i, j] == Attributes.FreePlace)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public List<Seat> GiveBestCluster()
        {
            return _clusterList.Values.FirstOrDefault(x => x.Count == _neededPlaces);
        }
    }
}
