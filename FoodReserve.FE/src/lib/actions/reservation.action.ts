"use server"

import { Reservation } from "../domains/reservation.domain";

export async function createReservation(reservation: Reservation) {
    try {
        await fetch(`${process.env.API_BASE_URL}/api/reservation`, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(reservation)
        });
    } catch (error) {
        console.error('Error creating reservation:', error);
        throw error;
    }
}