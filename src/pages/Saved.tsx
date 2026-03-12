import Navbar from "../components/Navbar";
import SavedRecipes from "../components/SavedRecipes";
import SavedHeadBar from "../components/SavedHeadBar";

const Saved = () => {
  return (
    <>
      <Navbar />
      <SavedHeadBar />
      <SavedRecipes />
    </>
  );
};

export default Saved;
