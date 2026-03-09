import type { RecipeType } from "../queryOptions/createRecipeQueryOption"
import styles from '../css/Recipes.module.css'
import { useNavigate } from "react-router-dom"

const RecipeCard = (recipe:RecipeType) => {
  const navigate = useNavigate()

  const goToForYou = () => {
    navigate("/foryou/" + recipe.id)
  }

  return (
      <div onClick={() => {goToForYou()}} className={styles.recipeCard}>
        <img className={styles.myImg} src={recipe.kepUrl ? recipe.kepUrl : "backgroud.jpg"} alt={recipe.nev} title={recipe.nev}/>
        <h2>{recipe.nev}</h2>
        <h3>{recipe.feltoltoUsername}</h3>
        <p>{recipe.elkeszitesiIdo} perc</p>
        <p>{recipe.nehezsegiSzint}</p>
        <div className={styles.tags}>
            {recipe.cimkek.map((c,idx) => <span key={idx} className={styles.tag}>{c}</span>)}
        </div>
        <div className={styles.cardFooter}>
            <span>{recipe.likes}❤️</span>
            <i className={recipe.mentveVan ? "fa-solid fa-bookmark" : "fa-regular fa-bookmark"}></i>
        </div>
    </div>
  )
}

export default RecipeCard