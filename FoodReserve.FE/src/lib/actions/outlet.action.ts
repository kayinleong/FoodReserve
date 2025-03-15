"use server"

import { Outlet } from "../domains/outlet.domain";
import { Pagination } from "../domains/pagination.domain";

export async function getAllOutlet(pageNumber: number, pageSize: number, keyword: string = ''): Promise<Pagination<Outlet>> {
    const res = await fetch(`${process.env.API_BASE_URL}/api/outlet?pageNumber=${pageNumber}&pageSize=${pageSize}&keyword=${keyword}`);
    return await res.json();
}

export async function getOutletById(id: string): Promise<Outlet> {
    const res = await fetch(`${process.env.API_BASE_URL}/api/outlet/${id}`);
    return await res.json();
}