import React, { useEffect, useRef, useState } from "react";
// @ts-ignore
import { useHistory, useParams } from "react-router-dom";
import { GeneratorConfiguration, GroupConfiguration } from "../../models/group-configuration";
import {
  Attention,
  Button,
  ButtonIcon,
  Grid,
  GridCell,
  GridContainer,
  Input,
  Label,
  Spinner
} from "optimizely-oui";
import { useServerSettingsContext } from "../../server-settings";
import { useDataServiceContext } from "../../data-service";
import { ContentLink } from "../../content-link";
import { GeneratorsList } from "./generators-list";

export interface EditConfigurationProps {
  onSaveSuccess: (message: string) => void;
}

export const EditConfiguration = ({ onSaveSuccess }: EditConfigurationProps) => {
  const { editContentLink } = useParams();
  const dataService = useDataServiceContext();
  const {
    availableNameGenerators = [],
    options: {
      databaseConfigurationsEnabled = true
    }
  } = useServerSettingsContext();

  const [isReadonly, setIsReadonly] = useState(!databaseConfigurationsEnabled);

  const history = useHistory();

  const [isLoading, setLoading] = useState(false);
  const [contentLink, setContentLink] = useState("");
  const [fromCode, setFromCode] = useState(false);
  const [contentExists, setIsContentExists] = useState<boolean | undefined>(false);
  const [changedBy, setChangedBy] = useState<string | undefined>("");
  const [changedOn, setChangedOn] = useState<string | undefined>("");
  const [generators, setGenerators] = useState<GeneratorConfiguration[]>([]);

  const contentLinkElementId = "content-link-" + new Date().getTime();
  const contentLinkElRef = useRef(null);

  const [validationMessage, setValidationMessage] = useState("");

  useEffect(() => {
    if (editContentLink) {
      setLoading(true);
      dataService?.get(editContentLink).then((result: GroupConfiguration) => {
        if (!result) {
          return;
        }
        setContentLink(result.contentLink);
        setFromCode(result.fromCode);
        setGenerators(result.groupLevelConfigurations || []);
        setIsReadonly(!databaseConfigurationsEnabled || result.fromCode);
        setIsContentExists(result.contentExists);
        setChangedBy(result.changedBy);
        setChangedOn(result.changedOn);
        setLoading(false);
      });
    } else {
      setGenerators([getGenerator(availableNameGenerators[0])]);
    }
  }, [editContentLink, dataService, databaseConfigurationsEnabled, availableNameGenerators]);

  const getGenerator = (generatorType: string): GeneratorConfiguration => {
    generatorType = generatorType.toLocaleLowerCase();

    let settings = undefined;
    switch (generatorType.toLocaleLowerCase()) {
      case "name":
        settings = {
          startIndex: "0",
          countLetters: "1",
          defaultName: "!default"
        };
        break;
      case "create date": {
        settings = {
          dateFormat: "yyyy",
          defaultValue: "!default"
        };
        break;
      }
    }

    return { name: generatorType, settings };
  };

  const onAddGenerator = () => {
    const updatedList = [...generators, getGenerator(availableNameGenerators[0])];
    setGenerators(updatedList);
  };

  const onGeneratorSettingsChanged = (index: number, settings: Record<string, string>) => {
    let updatedList = [...generators];
    updatedList[index].settings = settings;
    setGenerators(updatedList);
  };

  const onRemoveGenerator = (index: number) => {
    let updatedList = [...generators];
    updatedList.splice(index, 1);
    setGenerators(updatedList);
  };

  const onGeneratorValueChange = (index: number, value: string) => {
    let updatedList = [...generators];
    updatedList[index] = getGenerator(value);
    setGenerators(updatedList);
  };

  const isValid = (): boolean => {
    if (!contentLink || contentLink.trim() === "") {
      return false;
    }

    return (generators || []).length !== 0;
  };

  const onSave = () => {
    dataService
      .save({
        isNew: !isEditing,
        contentLink: contentLink || editContentLink,
        fromCode: false,
        groupLevelConfigurations: generators
      })
      .then(() => {
        onSaveSuccess("Configuration saved");
        history.push("/");
      })
      .catch((error) => {
        setValidationMessage(error);
      });
  };

  const selectContent = () => {
    const callback = (value: any) => {
      if (value) {
        // @ts-ignore
          setContentLink(contentLinkElRef?.current?.value || "");
      }
    };
    // @ts-ignore
    let epi = window.EPi;
    const url = epi.ResolveUrlFromUI("edit/pagebrowser.aspx");
    epi.CreatePageBrowserDialog(
      url,
      contentLink,
      true /*disableCurrentPageOption*/,
      false /*displayWarning*/,
      "hiddenField1" /*info*/,
      contentLinkElementId /*value*/,
      null /*language*/,
      callback /*callbackMethod*/,
      null /*callbackArguments*/,
      true /*requireUrlForSelectedPage*/
    );
  };

  const isEditing = !!editContentLink;

  if (isLoading) {
    return <Spinner />;
  }

  const RightContainer = () => {
    return <ButtonIcon iconName="ellipsis" onClick={selectContent} size="small" style="outline" title="Close Dialog" />;
  };

  return (
    <GridContainer className="edit-configuration plugin-grid">
      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <h2>{isEditing ? "Edit configuration" : "Add configuration"}</h2>
        </GridCell>

        {validationMessage && (
          <GridCell large={12} medium={8} small={4}>
            <Attention alignment="center" type="bad-news">
              {validationMessage}
            </Attention>
          </GridCell>
        )}

        {fromCode && (
          <GridCell large={12} medium={8} small={4}>
            <div className="text-description">Configuration registered from code cannot be edited</div>
          </GridCell>
        )}

        {!isEditing && (
          <GridCell className="content-link-select" large={12} medium={8} small={4}>
            <Input
              id="edit-configuration-content-link"
              displayError={false}
              type="text"
              isOptional={false}
              label="Container Content link"
              maxLength={5}
              className="content-link"
              min={1}
              onChange={(e) => setContentLink(e.target.value)}
              value={contentLink}
              isRequired
              isDisabled={isReadonly}
              RightContainer={RightContainer}
            />
            <input type="hidden" id="hiddenField1" />
            <input type="hidden" ref={contentLinkElRef} id={contentLinkElementId} />
          </GridCell>
        )}

        {isEditing && (
          <GridCell large={12} medium={8} small={4}>
            <label>
              {fromCode && <>Content link&nbsp;</>}
              {!fromCode && <>Editing content link&nbsp;</>}
              <span style={{ fontWeight: "bold" }}>
                <ContentLink value={editContentLink} contentExists={contentExists} />
              </span>
            </label>
          </GridCell>
        )}
        <GridCell large={12} medium={8} small={4}>
          <Label>Name generators *</Label>
          <GeneratorsList
            generators={generators}
            isReadonly={isReadonly}
            onGeneratorValueChange={onGeneratorValueChange}
            onRemoveGenerator={onRemoveGenerator}
            onAddGenerator={onAddGenerator}
            onSettingsChanged={onGeneratorSettingsChanged}
            availableNameGenerators={availableNameGenerators}
            databaseConfigurationsEnabled={databaseConfigurationsEnabled}
          />
        </GridCell>

        {isEditing && !fromCode && (
          <GridCell large={12} medium={8} small={4}>
            <div>
              Changed by: <span style={{ fontWeight: "bold" }}>{changedBy}</span>
            </div>
            <div>
              Changed on: <span style={{ fontWeight: "bold" }}>{changedOn}</span>
            </div>
          </GridCell>
        )}

        {!isReadonly && (
          <GridCell large={12} medium={8} small={4}>
            <Button style="plain" key={0} onClick={() => history.push("/")}>
              Cancel
            </Button>
            &nbsp;
            <Button isDisabled={!isValid()} style="highlight" key={1} onClick={onSave}>
              Save
            </Button>
          </GridCell>
        )}
        {isReadonly && (
          <GridCell large={12} medium={8} small={4}>
            <Button style="plain" key={0} onClick={() => history.push("/")}>
              Back
            </Button>
          </GridCell>
        )}
      </Grid>
    </GridContainer>
  );
};
//TODO: add resources
