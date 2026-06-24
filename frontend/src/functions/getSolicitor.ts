import { SolicitorInfo } from "../models/solicitorSummary";
import api from "./axios";

export default async function GetSolicitor(id : string) : Promise<SolicitorInfo | undefined> {
    const response = await api.get(`/solicitors/${id}`);
    return response.data as SolicitorInfo;
}