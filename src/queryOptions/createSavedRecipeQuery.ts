import { queryOptions } from "@tanstack/react-query";
import axios from "axios";
import type { CommentType, RecipeType } from "../types/RecipeTypes";

const api = axios.create({
  baseURL: "https://cbnncff2-7114.euw.devtunnels.ms",
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("access");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  config.headers["X-Tunnel-Skip-Anti-Phishing-Scan"] = "true"; 
  return config;
});


export default function createRecipeQueryOption() {
    return queryOptions({
        queryKey: ['recipes', 'me', 'saved'],
        queryFn: getRecipes
    })
}

export function createNextRecipeQueryOption(id: string) {
    return queryOptions({
        queryKey: ['next', id],
        queryFn: () => getNextRecipe(id)
    })
}

export function createRegisterQueryOption(username: string, password: string) {
    return queryOptions({
        queryKey: ['register', username, password],
        queryFn: () => register(username, password)
    })
}

export function createRecipeCommentsQueryOption(id: string) {
    return queryOptions({
        queryKey: ['comments', id],
        queryFn: () => getRecipeComments(id)
    })
}

export const getRecipes = async () : Promise<RecipeType[]> => {
  const response = await api.get("/api/user/me/saved")
  return await response.data
}

export const getNextRecipe = async (id: string) : Promise<RecipeType> => {
  const response = await api.get(`/api/Recept/${id}/next`)
  return await response.data
}

export const getPreviusRecipe = async (id: string) : Promise<RecipeType> => {
  const response = await api.get(`/api/Recept/${id}/next?direction=prev`)
  return await response.data
}

export const getRecipeComments = async (id: string) : Promise<CommentType[]> => {
  const response = await api.get(`/api/Komment/recept/${id}`)
  return await response.data
}

const register = async (username: string, password: string) => {
    const response = await api.post("/api/Auth/register", {username, password})
    return await response.data
}