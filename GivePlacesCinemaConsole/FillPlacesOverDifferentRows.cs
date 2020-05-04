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
        private List<List<Seat>> clusterList = new List<List<Seat>>();


        public FillPlacesOverDifferentRows(int neededPlaces, int rows, int columns, string[,] allPlaces)
        {
            _neededPlaces = neededPlaces;
            _rows = rows;
            _columns = columns;
            _allPlaces = allPlaces;
            middleRow = Math.Ceiling((decimal)rows / 2) - 1;
            middleColumn = Math.Ceiling((decimal)columns / 2) - 1;
            _bufferAllPLaces = _allPlaces;

            Go();
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

            placesRemaining--;

            var listOfFreeSeatsSameRow = ListOfFreeSeatsNextToSeat(freeSeat, placesRemaining);

            placesRemaining -= listOfFreeSeatsSameRow.Count;

            FillBufferAllPlaces(listOfFreeSeatsSameRow);

            // Dan naar de columns kijken op de rij + 1. clusters zoeken die gescheiden zijn.

            FindFreePlacesAboveRow(freeSeat.row, listOfFreeSeatsSameRow, placesRemaining);

            // Dan naar de columns kijken op de rij - 1. clusters zoeken die gescheiden zijn.

            FindFreePlacesBellowRow(freeSeat.row,listOfFreeSeatsSameRow, placesRemaining);
            // Dan naar de columns kijken op de rij + 2. clusters zoeken die gescheiden zijn.
            // enz
            // volledige cluster gevonden? opslagen en naar volgende
            // Slechte clusters ook opslagen zodat er niet onnodig gezocht wordt. 
            // Indien vrije seat in 1 van deze clusters? naar volgende. Altijd deze check doen, ook als je clusters aan het samenstellen bent.
            // Op het einde beste cluster kiezen => zo weinig mogelijk rijen, zo veel mogelijk in het midden (gemiddeld nummer collumns moet zo dicht mogelijk bij middelste liggen. Zelfde bij rij)

        }

        private Seat FindOneFreePlace()
        {
            var freeSeat = new Seat();

            for (int i = (int)middleRow; i >= 0; i--)
            {
                for (int j = (int)middleColumn; j <= (_columns - 1); j++)
                {
                    if (_allPlaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }

                for (int j = (int)middleColumn; j >= 0; i--)
                {
                    if (_allPlaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }
            }

            for (int i = (int)middleRow; i < _rows; i++)
            {
                for (int j = (int)middleColumn; j <= (_columns - 1); j++)
                {
                    if (_allPlaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }

                for (int j = (int)middleColumn; j >= 0; i--)
                {
                    if (_allPlaces[i, j] == freePlace)
                    {
                        freeSeat.row = i;
                        freeSeat.column = j;
                        return freeSeat;
                    }
                }
            }

            return null;
        }

        private List<Seat> ListOfFreeSeatsNextToSeat(Seat seat,int neededPlaces)
        {
            var listOfSeats = new List<Seat>();

            for (int i = 1; i < _columns - seat.column; i++)
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
                if (_allPlaces[seat.row, seat.column - i] == occupiedPlace)
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

            return listOfSeats;
        }

        private void FindFreePlacesAboveRow(int row, List<Seat> seatsOld, int placesRemaining)
        {
            var seatsNew = new List<Seat>();

            foreach (var seat in seatsOld)
            {
                seatsNew.Add(seat);
            }

            foreach (var seat in seatsNew.Where(s => s.row == row))
            {
                var seatAbove = seat;
                seatAbove.row++;

                if (_allPlaces[seatAbove.row, seatAbove.column] == freePlace)
                {
                    var listOfFreeSeats = ListOfFreeSeatsNextToSeat(seatAbove,placesRemaining);
                    FindFreePlacesAboveRow(row++, seatsNew, placesRemaining - listOfFreeSeats.Count);
                }
            }
        }

        private void FindFreePlacesBellowRow(int row, List<Seat> seatsOld, int placesRemaining)
        {
            var seatsNew = new List<Seat>();

            foreach (var seat in seatsOld)
            {
                seatsNew.Add(seat);
            }

            foreach (var seat in seatsNew.Where(s => s.row == row))
            {
                var seatBellow = seat;
                seatBellow.row--;

                if (_allPlaces[seatBellow.row, seatBellow.column] == freePlace)
                {
                    var listOfFreeSeats = ListOfFreeSeatsNextToSeat(seatBellow, placesRemaining);
                    FindFreePlacesBellowRow(row--,seatsNew, placesRemaining - listOfFreeSeats.Count);
                }
            }
        }

        private void FillBufferAllPlaces(List<Seat> seats)
        {
            foreach (var seat in seats)
            {
                _bufferAllPLaces[seat.row, seat.column] = occupiedPlace;
            }
        }
    }
}
