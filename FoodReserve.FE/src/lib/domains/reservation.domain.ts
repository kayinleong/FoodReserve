export interface Reservation {
    user_id: string | null;
    outlet_id: string;
    name: string;
    phone_number: string;
    number_of_guest: number;
    status: number;
    date: string;
}