import api from "./axios";

export default async function GetCiries() {
    const response = await api.get('/ratingsProviders');
    return response.data as string[];
}