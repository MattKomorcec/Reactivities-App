import React, { useContext, useState } from "react";
import { Tab, Grid, Header, Button, Form } from "semantic-ui-react";
import { RootStoreContext } from "../../app/stores/rootStore";
import TextInput from "../../app/common/form/TextInput";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import { Form as FinalForm, Field } from "react-final-form";
import {
  combineValidators,
  isRequired,
  hasLengthGreaterThan
} from "revalidate";
import { observer } from "mobx-react-lite";

const validate = combineValidators({
  displayName: isRequired({ message: "Display name is required" })
});

const ProfileAbout = () => {
  const rootStore = useContext(RootStoreContext);
  const {
    profile,
    isCurrentUser,
    updateProfile,
    updatingProfile
  } = rootStore.profileStore;
  const [updateProfileMode, setUpdateProfileMode] = useState(false);

  const initialValues = {
    displayName: profile!.displayName,
    bio: profile!.bio
  };

  const handleFormSubmit = (values: any) => {
    updateProfile(values).then(() => {
      setUpdateProfileMode(!updateProfileMode);
    });
  };

  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16} style={{ paddingBottom: 0 }}>
          <Header
            floated="left"
            icon="user"
            content={`About ${profile!.displayName}`}
          />
          {isCurrentUser && (
            <Button
              floated="right"
              basic
              content={updateProfileMode ? "Cancel" : "Edit Profile"}
              onClick={() => setUpdateProfileMode(!updateProfileMode)}
            />
          )}
        </Grid.Column>
      </Grid>

      <Grid>
        <Grid.Column width={16}>
          {!updateProfileMode && <p>{profile!.bio}</p>}

          {isCurrentUser && updateProfileMode && (
            <FinalForm
              validate={validate}
              initialValues={initialValues}
              onSubmit={handleFormSubmit}
              render={({ handleSubmit, invalid, pristine }) => (
                <Form onSubmit={handleSubmit} loading={updatingProfile}>
                  <Field
                    placeholder="Display name"
                    value={profile!.displayName}
                    name="displayName"
                    component={TextInput}
                  />
                  <Field
                    placeholder="Bio"
                    value={profile!.bio}
                    name="bio"
                    component={TextAreaInput}
                    rows={3}
                  />
                  <Button
                    content="Update profile"
                    positive
                    floated="right"
                    loading={updatingProfile}
                    disabled={updatingProfile || invalid || pristine}
                  />
                </Form>
              )}
            />
          )}
        </Grid.Column>
      </Grid>
    </Tab.Pane>
  );
};

export default observer(ProfileAbout);
