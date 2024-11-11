import Layout from "../components/Layout";
import { Typography } from "antd";
import Link from "next/link";
import paths from "../paths";
import Image from "next/image";
import overviewImage from "../public/overview.png";
import germanFederalMinistry from "../public/german-federal-ministry-for-economic-affairs-and-climate-action.png";

function Page() {
  return (
    <Layout>
      <div style={{ maxWidth: 768 }}>
        <Typography.Paragraph>
          <a href="#manufacturers">For Manufacturers</a> |{" "}
          <a href="#software-companies">For Software Companies</a> |{" "}
          <a href="#architects-planners-engineers">For Architects, Planners, and Engineers</a>
        </Typography.Paragraph>
        <Typography.Paragraph>
          <Link href={paths.home}>buildingenvelopedata.org</Link> offers you
          access to a network of <Link href={paths.databases}>databases</Link>.
          It contains detailed <Link href={paths.data}>optical data</Link> for
          thousands of building envelope{" "}
          <Link href={paths.components}>components</Link> and can be used for
          example to calculate the energy performance of buildings. The{" "}
          <Link href={paths.data}>data</Link> is ready to be used by software
          companies and advanced engineering offices.
        </Typography.Paragraph>
        <Typography.Paragraph>
          This website offers an overview of the{" "}
          <Link href={paths.components}>components</Link> for which{" "}
          <Link href={paths.data}>data</Link> is available and the{" "}
          <Link href={paths.databases}>databases</Link> of the network. It can
          also be used to search for <Link href={paths.data}>data</Link> in all{" "}
          <Link href={paths.databases}>databases</Link>. In order to identify{" "}
          <Link href={paths.institutions}>institutions</Link>,{" "}
          <Link href={paths.dataFormats}>data formats</Link> and{" "}
          <Link href={paths.methods}>methods</Link> across the{" "}
          <Link href={paths.databases}>databases</Link>, they are managed by a
          metabase together with the{" "}
          <Link href={paths.components}>components</Link> and{" "}
          <Link href={paths.databases}>databases</Link>. This website is the
          front end of the metabase.
        </Typography.Paragraph>
        <Typography.Paragraph>
          The metabase can be queried through its{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
          >
            GraphQL endpoint
          </Typography.Link>
          . This is the most powerful way to query all{" "}
          <Link href={paths.databases}>databases</Link>. It is well suited to be
          used by software. The tabs of this website can offer only a part of
          the functions of the{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
          >
            GraphQL endpoint
          </Typography.Link>
          .
        </Typography.Paragraph>
        <Typography.Paragraph>
          With{" "}
          <Typography.Link
            href={paths.dataFormat("9ca9e8f5-94bf-4fdd-81e3-31a58d7ca708")}
          >
            BED-JSON
          </Typography.Link>
          , <Link href={paths.home}>buildingenvelopedata.org</Link> offers a
          general format for optical, calorimetric and photovoltaic data sets.
          It is defined by the JSON Schemas of the{" "}
          <Typography.Link href="https://github.com/building-envelope-data/api">
            buildingenvelopedata.org API specification
          </Typography.Link>
          . Other data formats are also available. The{" "}
          <Typography.Link href="https://github.com/building-envelope-data/metabase">
            source code
          </Typography.Link>{" "}
          of the metabase is available at{" "}
          <Typography.Link href="https://github.com">GitHub</Typography.Link>.
        </Typography.Paragraph>
        <Image
          src={overviewImage}
          alt="Schematic depiction of how users like architects, planners, or engineers can use the metabase to find products and data in and across databases."
          style={{
            maxWidth: "100%",
            height: "auto",
          }} />
        <Typography.Title level={2} id="manufacturers" >For Manufacturers</Typography.Title>
        <Typography.Paragraph>
         <strong>Manufacturers of building envelope components often face the following challenges:</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          They receive product data from various sources, for example laboratories or suppliers. There are several types of product data, but also several formats for one type of product data. 
          The manufacturer selects a part of the product data and submits it to databases of associations e.g., the International Glazing and Shading Database (IGSDB) and the European Solar-Shading Database (ES-SDA), in the format which they require.
        </Typography.Paragraph>
        <Typography.Paragraph>
          There is a large variety of software applications used by architects and engineers (“planners”) to design building envelopes. 
          Several planning applications have a limited amount of product data stored within the application. 
          The manufacturer must decide which data is provided to which application and which overall costs are acceptable as part of the marketing of the products. 
          Most manufacturers have a website where planners can download datasheet values of many components. 
          Planners need to type them manually into their planning application which is time-consuming and error-prone. 
          When planners have chosen interesting components of a manufacturer, they want that their planning software can directly simulate with these components. 
          They don’t want to research which calculation method requires which type of data in which format. 
          Manufacturers must keep their product data in sync across all databases of associations, planning applications and their own website.
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>Solutions with the product data network</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          The product data network buildingenvelopedata.org provides the format BED-JSON which covers all optical and calorimetric data which is currently relevant. 
          Instead of exchanging product data manually, manufacturers add an additional application programming interface to their existing website server, or their association adds the additional API to their existing website server. 
          The product data network provides a specification for this API. Planning applications can access the product data in an automated way which reduces the cost for managing the product data significantly.
        </Typography.Paragraph>
        <Typography.Paragraph>
          Planners prefer to work with building envelope components which they can easily simulate in their favorite application. 
          <strong>The product data network connects your product data with many software applications and with the architects, planners and engineers who use the planning software. </strong> 
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>Next steps</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          There is a {" "}<Typography.Link href="https://github.com/building-envelope-data#how-can-i-add-my-product-data-to-the-product-data-network">description for software developers how to connect your product data to the planning software applications.</Typography.Link>{" "}
          Please {" "} <Typography.Link href="https://github.com/building-envelope-data/api/issues/new">
            raise an issue
          </Typography.Link> {" "} if we can help you in any way.
        </Typography.Paragraph>
        <Typography.Title level={2} id="software-companies" >For software companies</Typography.Title>
        <Typography.Paragraph>
         <strong>Software companies which develop planning applications for buildings and building envelopes face several challenges:</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          Optical and calorimetric data is typically available either as a PDF datasheet or in a special text format for experts in these domains. 
          Software companies therefore need software developers which become experts for such data. 
          If the information is provided as a PDF datasheet, it cannot be extracted reliably. 
          If the information is provided in a special text format, it is difficult to parse. Some data sets may not be valid e.g. because the decimal separator has been entered wrong.
        </Typography.Paragraph>
        <Typography.Paragraph>
          Planners must comply with the local building code, which means that their planning software must comply with the local regulations, too. 
          For example, the definition of solar transmittance according to EN 410 differs from that in ISO 9050. 
          The required quality of the product data depends on the required calculation. 
          Optical and calorimetric data is typically provided without specifying the method according to which they were created. 
          Therefore, software companies and planners sometimes do not know whether they can use the data which they have found for the intended purpose.
        </Typography.Paragraph>
        <Typography.Paragraph>
          Planners want to be able to use all available components in their planning software. 
          It is time-consuming to collect product data manually from manufacturers.
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>Solutions with the product data network</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          The product data network buildingenvelopedata.org allows software companies to download product data from several manufacturers and associations via the same application programming interface (API). 
          The format BED-JSON was developed explicitly for software developers without background in optical or calorimetric data. 
          The JSON schemas explain each key and can be used to validate data sets. It is simple to parse the JSON data set and to use it in any programming language.
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>The product data network provides an easy access to a large amount of product data which is always up to date.</strong> 
          Your planning software can use it to make the planning of building envelopes easier, faster, and better. Architects, planners, and engineers don't need to understand all details of the calculation and the required data. 
          They appreciate applications which make it easy to evaluate many variants in a short time to find the best solution for their customers.
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>Next steps</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          There is a {" "}<Typography.Link href="https://github.com/building-envelope-data#how-can-i-use-the-product-data-network-with-my-software-application">description for software developers how to use the product data network with your application.</Typography.Link>{" "}
          Please {" "} <Typography.Link href="https://github.com/building-envelope-data/api/issues/new">
            raise an issue
          </Typography.Link> {" "} if we can help you in any way.
        </Typography.Paragraph>
        <Typography.Title level={2} id="architects-planners-engineers" >For architects, planners and engineers</Typography.Title>
        <Typography.Paragraph>
         <strong>There are at least eight challenges for planners affecting the speed of their work, the required effort, and the quality of the results:</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          Planning software typically contains a limited amount of product data. 
          When planners need to evaluate other products, they need to understand which data and format is required. 
          The manual search for product data is time-consuming. Therefore, only some products can be evaluated. 
          It is difficult to find the components which are suited best to a specific application. Product data often needs to be entered manually into planning software, which is time-consuming and prone to error. 
          When many variants of the building envelope are compared, effort is needed to document the inputs of each simulation properly. 
          Time and effort are needed to get product data from manufacturers, especially when detailed data is needed. 
          Planners sometimes receive the required data content in a format which cannot be processed by their planning software. 
          To get the required product data in the required format, several software applications can be necessary. 
          Processes with several applications can be error-prone and cumbersome to maintain.
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>Solutions with the product data network</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          The product data network buildingenvelopedata.org adds a large amount of product data to your favorite planning software. 
          You don’t need to understand all details of the calculations. 
          You don’t need to understand which format is required by the planning software, where you find the product data and which data sets are suitable for the calculation.
        </Typography.Paragraph>
        <Typography.Paragraph>
          Your planning software makes it easy for you by using the product data network. You can quickly evaluate all interesting variants and find great solutions for your customers.
        </Typography.Paragraph>
        <Typography.Paragraph>
          <strong>Next steps</strong>
        </Typography.Paragraph>
        <Typography.Paragraph>
          If your favorite planning software does not yet provide a large amount of glass panes and shading to choose from, ask them to connect to the product data network. 
          Software companies can use the product data for free.
        </Typography.Paragraph>
        <Typography.Title level={2}>Acknowledgements</Typography.Title>
        <Typography.Paragraph>
          This work was funded by the German Federal Ministry for Economic Affairs and Climate Action under Grants 03ET1560A and 03EN1070A, based on a decision by the German Bundestag, by a Fraunhofer ICON Grant and by the Assistant Secretary for Energy Efficiency and Renewable Energy, Building Technologies Program, of the U.S. Department of Energy, under Contract No. DE-AC02-05CH11231.
        </Typography.Paragraph>
        <Image
          src={germanFederalMinistry}
          alt="Coat of arms of Germany with the text: Supported by the German Federal Ministry for Economic Affairs and Climate Action on the basis of a decision by the German Bundestag."
          style={{
            maxWidth: "50%",
            height: "auto",
          }} />
      </div>
    </Layout>
  );
}

export default Page;
