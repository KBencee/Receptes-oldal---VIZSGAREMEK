import { queryOptions } from "@tanstack/react-query";
import axios from "axios";
import type { CommentType, RecipeType } from "../types/RecipeTypes";

const BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms"

export default function createRecipeQueryOption() {
    return queryOptions({
        queryKey: ['recipes'],
        queryFn: getRecipes
    })
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