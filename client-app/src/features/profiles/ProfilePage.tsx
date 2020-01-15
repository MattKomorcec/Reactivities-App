import React, { useContext, useEffect } from "react";
import ProfileHeader from "./ProfileHeader";
import { Grid } from "semantic-ui-react";
import ProfileContent from "./ProfileContent";
import { RootStoreContext } from "../../app/stores/rootStore";
import { RouteComponentProps } from "react-router-dom";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { observer } from "mobx-react-lite";

interface IRouteParams {
  username: string;
}

interface IProps extends RouteComponentProps<IRouteParams> {}

const ProfilePage: React.FC<IProps> = ({ match }) => {
  const rootStore = useContext(RootStoreContext);
  const {
    profile,
    loadingProfile,
    loadProfile,
    follow,
    unfollow,
    isCurrentUser,
    loading,
    setActiveTab
  } = rootStore.profileStore;

  useEffect(() => {
    loadProfile(match.params.username);
  }, [loadProfile, match]);

  if (loadingProfile) {
    return <LoadingComponent content="Loading profile..." />;
  }

  return (
    <Grid>
      <Grid.Column width={16}>
        <ProfileHeader
          profile={profile!}
          follow={follow}
          unfollow={unfollow}
          isCurrentUser={isCurrentUser}
          loading={loading}
        />
        <ProfileContent setActiveTab={setActiveTab} />
      </Grid.Column>
    </Grid>
  );
};

export default observer(ProfilePage);
