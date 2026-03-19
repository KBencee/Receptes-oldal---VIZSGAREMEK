import styles from "../css/Recipes.module.css";
import { useNavigate } from "react-router-dom";
import type { RecipeType } from "../types/RecipeTypes";

const RecipeCard = (recipe: RecipeType) => {
const navigate = useNavigate();

const goToForYou = () => {
	navigate("/foryou/" + recipe.id);
};

return (
	<div className={styles.recipeCard} onClick={() => {goToForYou()}}>
		<img
			className={styles.myImg}
			src={recipe.kepUrl ? recipe.kepUrl : "backgroud.jpg"}
			alt={recipe.nev}
			title={recipe.nev}
		/>
		<h2>{recipe.nev}</h2>
		<h3>{recipe.feltoltoUsername}</h3>
		<p>{recipe.elkeszitesiIdo} perc</p>
		<p>{recipe.nehezsegiSzint}</p>
		<div className={styles.tags}>
			{recipe.cimkek.map((c, idx) => (
			<span key={idx} className={styles.tag}>
				{c}
			</span>
			))}
		</div>
		<div className={styles.cardFooter}>
			<span>{recipe.likes}❤️</span>
			{recipe.mentveVan ?
				<i className="fa-solid fa-bookmark"></i>
			:
				<i className="fa-regular fa-bookmark"></i>
			}
		</div>
	</div>
);
};

export default RecipeCard;
