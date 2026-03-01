import api from "./axios";

export async function register(firstName, lastName, email, password) {
  const response = await api.post("/api/auth/register", {
    firstName, lastName, email, password
  });
  return response.data;
}