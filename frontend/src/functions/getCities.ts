import api from "./axios";

export default async function GetCiries() {
    const response = await api.get('/cities');
    return response.data as string[];
}