import { queryOptions } from "@tanstack/react-query";
import axios from "axios";
import type { CommentType, RecipeType } from "../types/RecipeTypes";
import { BASE_URL } from "../services/publicAPI";

const api = axios.create({
  baseURL: BASE_URL,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("access");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  config.headers["X-Tunnel-Skip-Anti-Phishing-Scan"] = "true"; 
  return config;
});

export function createRecipeQueryOption() {
    return queryOptions({
        queryKey: ['recipes'],
        queryFn: getRecipes
    })
}

export function createOwnRecipeQueryOption() {
    return queryOptions({
        queryKey: ['recipes', 'me'],
        queryFn: getOwnRecipes
    })
}

export function createSavedRecipeQueryOption() {
    return queryOptions({
        queryKey: ['recipes', 'me', 'saved'],
        queryFn: getSavedRecipes
    })
}

export const searchRecipesByTitle = async (term: string): Promise<RecipeType[]> => {
  const response = await api.get("/api/Recept/search", {
    params: { query : term }
  })
  return response.data
}

export const searchRecipesByTags = async (targetId: number): Promise<RecipeType[]> => {
    const response = await api.get(`/api/Recept/by-tag/${targetId}`)
    return response.data
}

export function createRecipeByIdQueryOption(id: string) {
    return queryOptions({
        queryKey: ['byId', id],
        queryFn: () => getRecipeById(id)
    })
}
export function createNextRecipeQueryOption(id: string) {
    return queryOptions({
        queryKey: ['next', id],
        queryFn: () => getNextRecipe(id)
    })
}

export function createPrevRecipeQueryOption(id: string) {
    return queryOptions({
        queryKey: ['prev', id],
        queryFn: () => getPrevRecipe(id)
    })
}

export function createRecipeCommentsQueryOption(id: string) {
    return queryOptions({
        queryKey: ['comments', id],
        queryFn: () => getRecipeComments(id)
    })
}

const getRecipes = async () : Promise<RecipeType[]> => {
  const response = await axios.get(BASE_URL + "/api/Recept")
  return await response.data
}

const getOwnRecipes = async () : Promise<RecipeType[]> => {
  const response = await api.get("/api/Recept/my-recipes")
  return await response.data
}

const getSavedRecipes = async () : Promise<RecipeType[]> => {
  const response = await api.get("/api/user/me/saved")
  return await response.data
}

const getRecipeById = async (id: string) : Promise<RecipeType> => {
  const response = await axios.get(BASE_URL + `/api/Recept/${id}`)
  return await response.data
}
const getNextRecipe = async (id: string) : Promise<RecipeType> => {
  const response = await axios.get(BASE_URL + `/api/Recept/${id}/next`)
  return await response.data
}

const getPrevRecipe = async (id: string) : Promise<RecipeType> => {
  const response = await axios.get(BASE_URL + `/api/Recept/${id}/next?direction=prev`)
  return await response.data
}

const getRecipeComments = async (id: string) : Promise<CommentType[]> => {
  const response = await axios.get(BASE_URL + `/api/Komment/recept/${id}`)
  return await response.data
}