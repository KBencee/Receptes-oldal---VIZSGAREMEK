import {  useLocation, useNavigate } from "react-router-dom"
import { createNextRecipeQueryOption, createPrevRecipeQueryOption, type RecipeType } from "../queryOptions/createRecipeQueryOption"
import { useMobileContext } from "../context/MobileContextProvider"
import { useState } from "react"
import { useQuery } from "@tanstack/react-query"
import styles from '../css/ForYou.module.css'
import ForYouReel from "../components/ForYouReel"
import Description from "../components/Description"

const ForYou = () => {
    const {isMobile} = useMobileContext()
    const location = useLocation()

    const state = location.state.recipe
    const [recipe, setRecipe] = useState<RecipeType>(state as RecipeType)

    const next = useQuery(createNextRecipeQueryOption(recipe.id))
    const prev = useQuery(createPrevRecipeQueryOption(recipe.id))

    const navigate = useNavigate()
    const nextFuncion = () => {
        navigate("/foryou", {state: {recipe}})
        setRecipe(next.data as RecipeType)
        console.log("next: " + next.data?.nev);
    }
    const prevFuncion = () => {
        navigate("/foryou", {state: {recipe}})
        setRecipe(prev.data as RecipeType)
        console.log("prev: " + prev.data?.nev);
    }

    console.log("current: " + recipe.nev);
    

  return (
    <div className={styles.forYou}>
        <ForYouReel {...recipe}/>
        { !isMobile &&
            <>
                <div className={styles.arrows}>
                    <i className="fa-solid fa-circle-down fa-rotate-180 fa-xl" onClick={() => {nextFuncion()}}></i>
                    <i className="fa-solid fa-circle-down fa-xl" onClick={() => {prevFuncion()}}/>
                </div>
                <Description {...recipe}/>
            </>
        }
    </div>
  )
}

export default ForYou